using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Client.Utils.Classes;
using Client.Utils.Exceptions.ApplicationState;
using Client.Utils.Exceptions.Auth;
using Client.Utils.Exceptions.Network;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Client.Models.EmployeeManagement; 
using Shared.Responses.UserManagement;

namespace Client.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly ISecureTokenStorage _tokenStorage;
    private readonly IConfiguration _configuration;
    
    // --- Added Dependencies for Session Management ---
    private readonly ISessionService _sessionService;
    private readonly IEmployeeRepository _employeeRepository;
    // -----------------------------------------------

    private readonly string _clientId;
    private readonly string _redirectUri;
    private readonly string _clientSecret;
    private readonly SemaphoreSlim _authSemaphore = new(1, 1);
    
    private string? _appJwtToken;
    private DateTime _tokenExpiry = DateTime.MinValue;
    
    public string? CurrentUserEmail { get; private set; }
        
    public event EventHandler<AuthenticationChangedEventArgs>? AuthenticationChanged;
    public bool IsAuthenticated => !string.IsNullOrEmpty(_appJwtToken) && DateTime.UtcNow < _tokenExpiry;

    public AuthenticationService(
        IHttpClientFactory httpClientFactory, 
        ILogger<AuthenticationService> logger,
        ISecureTokenStorage tokenStorage, 
        IConfiguration configuration,
        ISessionService sessionService,      
        IEmployeeRepository employeeRepository 
        )
    {
        _logger = logger;
        _tokenStorage = tokenStorage;
        _configuration = configuration;
        _sessionService = sessionService;         
        _employeeRepository = employeeRepository; 
        _httpClient = httpClientFactory.CreateClient("OAuth");

        try
        {
            _clientId = _configuration["Auth:ClientId"]
                        ?? throw new ConfigurationException("Google OAuth ClientId not found.", "Config Error", "Auth:ClientId");
            
            _clientSecret = _configuration["Auth:ClientSecret"]
                            ?? throw new ConfigurationException("Google OAuth ClientSecret not found.", "Config Error", "Auth:ClientSecret");
            
            _redirectUri = _configuration["Auth:Google:RedirectUri"] 
                           ?? "http://localhost:8080/callback";
        }
        catch (Exception e) when (e is not ConfigurationException)
        {
            throw new ConfigurationException("Auth Config Error", "Contact Support", "Auth", e);
        }

        _ = Task.Run(async () =>
        {
            try
            {
                await LoadStoredTokenAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to load stored token during startup.");
            }
        });
    }
    
    public async Task<bool> LoginAsync()
    {
        await _authSemaphore.WaitAsync();
        try
        {
            _logger.LogInformation("Starting Google OAuth login flow.");

            var codeVerifier = GenerateCodeVerifier();
            var codeChallenge = GenerateCodeChallenge(codeVerifier);
            var state = GenerateSecureState();

            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));

            (string Code, string RedirectUri) result;
            try
            {
                result = await GetAuthorizationCodeAsync(codeChallenge, state, cts.Token);
            }
            catch (OperationCanceledException)
            {
                throw new AuthenticationTimeoutException("OAuth login timed out.");
            }

            if (string.IsNullOrEmpty(result.Code))
            {
                throw new AuthorizationException("Authorization code not received.");
            }
            
            var googleTokenResponse = await ExchangeCodeForTokensAsync(result.Code, codeVerifier, result.RedirectUri);
            
            // Exchange and get User Data
            var appAuthResponse = await ExchangeGoogleTokenForAppJwtAsync(googleTokenResponse.IdToken!);
            
            await StoreAppJwtAsync(appAuthResponse.JwtToken, appAuthResponse.ExpiresIn);

            // Set Email
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(appAuthResponse.JwtToken);
            var emailClaim = jsonToken.Claims.FirstOrDefault(c => 
                c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name" ||
                c.Type == "email" || c.Type == "sub"); 
            CurrentUserEmail = emailClaim?.Value;

            if (appAuthResponse.User != null)
            {
                _logger.LogInformation("Login successful. Initializing session for: {Email}", appAuthResponse.User.Email);
                
                // Fetch full employee details if ID exists
                Shared.EmployeeManagement.Responses.EmployeeResponse? employee = null;
                if (appAuthResponse.User.EmployeeId > 0)
                {
                    var empResult = await _employeeRepository.GetByIdAsync(appAuthResponse.User.EmployeeId);
                    if (empResult.IsSuccess)
                    {
                        employee = empResult.Value;
                    }
                }

                await _sessionService.InitializeSessionAsync(appAuthResponse.User, employee);
            }

            _logger.LogInformation($"Login completed. User: {CurrentUserEmail}");
            AuthenticationChanged?.Invoke(this, new AuthenticationChangedEventArgs(true));
            return true;
        }
        catch (AuthenticationTimeoutException)
        {
            _logger.LogWarning("OAuth login timed out.");
            throw;
        }
        catch (AuthorizationException)
        {
            _logger.LogWarning("OAuth authorization failed.");
            throw;
        }
        catch (TokenStorageException)
        {
            _logger.LogError("Failed to store authentication tokens.");
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error during OAuth login.");
            throw new AuthenticationException($"Unexpected error: {e.Message}", innerException: e);
        }
        finally
        {
            _authSemaphore.Release();
        }
    }

    public async Task LogoutAsync()
    {
        await _authSemaphore.WaitAsync();

        try
        {
            _logger.LogInformation("Logging out user.");

            try
            {
                await _tokenStorage.DeleteTokenAsync("app_jwt_token");
                await _tokenStorage.DeleteTokenAsync("app_jwt_expiry");
            }
            catch (Exception e)
            {
                throw new TokenStorageException("Failed to delete tokens.", "Sign out incomplete.", "delete", innerException: e);
            }

            _appJwtToken = null;
            _tokenExpiry = DateTime.MinValue;
            CurrentUserEmail = null;

            // Clear session data
            await _sessionService.ClearSessionAsync();

            AuthenticationChanged?.Invoke(this, new AuthenticationChangedEventArgs(false));
        }
        finally
        {
            _authSemaphore.Release();
        }
    }

    public async Task<string> GetAccessTokenAsync()
    {
        await _authSemaphore.WaitAsync();

        try
        {
            if (string.IsNullOrEmpty(_appJwtToken))
            {
                throw new AuthenticationException("No access token available.", "Not signed in.");
            }

            if (DateTime.UtcNow >= _tokenExpiry)
            {
                await LogoutAsync();
                throw new AuthenticationException("Session expired.", "Please sign in again.");
            }

            return _appJwtToken;
        }
        finally
        {
            _authSemaphore.Release();
        }
    }

    private async Task<(string Code, string RedirectUri)> GetAuthorizationCodeAsync(
        string codeChallenge, string expectedState, CancellationToken cancellationToken)
    {
        HttpListener? listener = null;

        try
        {
            var portsToTry = new[] { 8081, 8082, 8083, 8084, 8085 };
            string? callbackUrl = null;
            
            foreach (var port in portsToTry)
            {
                try
                {
                    callbackUrl = $"http://localhost:{port}/";
                    listener = new HttpListener();
                    listener.Prefixes.Add(callbackUrl);
                    listener.Start();
                    _logger.LogInformation("Successfully listening on: {CallbackUrl}", callbackUrl);
                    break;
                }
                catch (HttpListenerException)
                {
                    listener?.Close();
                    listener = null;
                    _logger.LogDebug("Port {Port} is in use, trying next port", port);
                }
            }

            if (listener == null || callbackUrl == null)
            {
                throw new AuthorizationException("Unable to start OAuth callback listener - all ports are in use.");
            }

            var authUrl = BuildAuthorizationUrl(codeChallenge, expectedState, callbackUrl.TrimEnd('/'));

            try
            {
                var psi = new ProcessStartInfo { FileName = authUrl, UseShellExecute = true };
                Process.Start(psi);
            }
            catch (Exception e)
            {
                throw new AuthorizationException("Failed to open browser.", "Unable to open browser.", innerException: e);
            }

            var context = await listener.GetContextAsync().WaitAsync(cancellationToken);
            await SendCallbackResponseAsync(context);

            var query = context.Request.Url?.Query;
            if (string.IsNullOrEmpty(query))
            {
                throw new AuthorizationException("No query parameters received.");
            }

            var queryParams = System.Web.HttpUtility.ParseQueryString(query);
            var code = queryParams["code"];
            var state = queryParams["state"];
            var error = queryParams["error"];

            if (!string.IsNullOrEmpty(error))
            {
                throw new AuthorizationException($"OAuth provider returned error: {error}");
            }

            if (state != expectedState)
            {
                throw new OAuthStateException("OAuth state mismatch.");
            }

            if (string.IsNullOrEmpty(code))
            {
                throw new AuthenticationException("Authorization code not received.");
            }

            return (code, callbackUrl.TrimEnd('/'));
        }
        finally
        {
            try
            {
                if (listener is { IsListening: true }) listener.Stop();
                listener?.Close();
            }
            catch { }
        }
    }
    
    private async Task SendCallbackResponseAsync(HttpListenerContext context)
    {
        const string html = """
                            <!DOCTYPE html>
                            <html>
                            <head>
                                <title>Authentication Complete</title>
                                <style>
                                    body { font-family: Arial, sans-serif; text-align: center; padding: 50px; }
                                    .success { color: #4CAF50; }
                                    .container { max-width: 400px; margin: 0 auto; }
                                </style>
                            </head>
                            <body>
                                <div class='container'>
                                    <h2 class='success'>✓ Authentication Successful</h2>
                                    <p>You can now close this window and return to the application.</p>
                                </div>
                                <script>
                                    setTimeout(function() { window.close(); }, 3000);
                                </script>
                            </body>
                            </html>
                            """;

        try
        {
            var buffer = Encoding.UTF8.GetBytes(html);
            context.Response.ContentType = "text/html";
            context.Response.ContentLength64 = buffer.Length;
            context.Response.StatusCode = 200;
            
            await context.Response.OutputStream.WriteAsync(buffer);
            context.Response.OutputStream.Close();
            context.Response.Close();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send callback response");
        }
    }

    private async Task<TokenResponse> ExchangeCodeForTokensAsync(string code, string codeVerifier, string redirectUri)
    {
        const string tokenEndpoint = "https://oauth2.googleapis.com/token";
        var parameters = new Dictionary<string, string>
        {
            ["client_id"] = _clientId,
            ["client_secret"] = _clientSecret,
            ["code"] = code,
            ["grant_type"] = "authorization_code",
            ["redirect_uri"] = redirectUri,         
            ["code_verifier"] = codeVerifier
        };

        using var content = new FormUrlEncodedContent(parameters);

        HttpResponseMessage? response = null;
        string responseBody = "";
        try
        {
            response = await _httpClient.PostAsync(tokenEndpoint, content);
            responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new AuthorizationException($"Token exchange failed: {response.StatusCode}");
            }

            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;

            var token = new TokenResponse
            {
                AccessToken = root.GetProperty("access_token").GetString(),
                IdToken = root.TryGetProperty("id_token", out var idt) ? idt.GetString() : null
            };

            if (string.IsNullOrEmpty(token.IdToken))
                throw new AuthorizationException("Token exchange returned no id_token.");

            return token;
        }
        catch (AuthorizationException) { throw; }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exchanging tokens.");
            throw new AuthorizationException("Unexpected error exchanging tokens.", innerException: ex);
        }
        finally
        {
            response?.Dispose();
        }
    }
    
    private async Task<AppAuthResponse> ExchangeGoogleTokenForAppJwtAsync(string googleIdToken)
    {
        var backendUrl = _configuration["CloudSyncUrl"] ?? throw new ConfigurationException("Backend URL missing.", "Config Error", "CloudSyncUrl");

        var request = new { IdToken = googleIdToken };

        using var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{backendUrl}api/auth/login", content);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new AuthenticationException($"Backend login failed. Status: {response.StatusCode} Body: {body}");
        }

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var appAuth = JsonSerializer.Deserialize<AppAuthResponse>(body, options);
        
        if (appAuth is null || string.IsNullOrEmpty(appAuth.JwtToken))
        {
            throw new AuthenticationException("Backend returned invalid response.");
        }

        return appAuth;
    }

    private async Task StoreAppJwtAsync(string jwt, int expiresIn)
    {
        _appJwtToken = jwt;
        _tokenExpiry = DateTime.UtcNow.AddSeconds(expiresIn);

        await _tokenStorage.StoreTokenAsync("app_jwt_token", jwt);
        await _tokenStorage.StoreTokenAsync("app_jwt_expiry", _tokenExpiry.ToString("o"));
        
        _logger.LogInformation("JWT stored. Expires at: {Expiry}", _tokenExpiry);
    }

    private async Task LoadStoredTokenAsync()
    {
        try
        {
            _appJwtToken = await _tokenStorage.RetrieveTokenAsync("app_jwt_token");
            var expiryString = await _tokenStorage.RetrieveTokenAsync("app_jwt_expiry");

            if (DateTime.TryParse(expiryString, out var expiry))
            {
                _tokenExpiry = expiry;
            }

            if (IsAuthenticated)
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(_appJwtToken);
                var emailClaim = jsonToken.Claims.FirstOrDefault(c => 
                    c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name" ||
                    c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" || 
                    c.Type == "email" || 
                    c.Type == "sub");
                CurrentUserEmail = emailClaim?.Value;

                _logger.LogInformation("Loaded valid stored JWT.");
                AuthenticationChanged?.Invoke(this, new AuthenticationChangedEventArgs(true));
            }
            else
            {
                await LogoutAsync();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error loading stored token.");
        }
    }
    
    private static string GenerateCodeVerifier()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static string GenerateCodeChallenge(string codeVerifier)
    {
        using var sha256 = SHA256.Create();
        var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
        return Convert.ToBase64String(challengeBytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static string GenerateSecureState()
    {
        var bytes = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }

    private string BuildAuthorizationUrl(string codeChallenge, string state, string redirectUri)
    {
        var scopes = Uri.EscapeDataString("openid email profile");
        return $"https://accounts.google.com/o/oauth2/v2/auth?" +
               $"client_id={Uri.EscapeDataString(_clientId)}&" +
               $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
               $"response_type=code&" +
               $"scope={scopes}&" +
               $"code_challenge={codeChallenge}&" +
               $"code_challenge_method=S256&" +
               $"state={state}&" +
               $"access_type=offline&" +
               $"prompt=consent";
    }

    public void Dispose()
    {
        _authSemaphore?.Dispose();
    }

    private class TokenResponse
    {
        public string? AccessToken { get; set; }
        public string? IdToken { get; set; }
    }
    
    private class AppAuthResponse
    {
        public string JsonWebToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; } = 3600;
        
        public UserResponse? User { get; set; }
        
        public string JwtToken 
        { 
            get => JsonWebToken; 
            set => JsonWebToken = value; 
        }
    }
}