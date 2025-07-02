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

namespace Client.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly ISecureTokenStorage _tokenStorage;
    private readonly IConfiguration _configuration;

    private readonly string _clientId;
    private readonly string _redirectUri;
    private readonly SemaphoreSlim _authSemaphore = new(1, 1);

    private string? _accessToken;
    private string? _refreshToken;
    private DateTime _tokenExpiry = DateTime.MinValue;
    private Timer? _refreshTimer;
        
    public event EventHandler<AuthenticationChangedEventArgs>? AuthenticationChanged;
    public bool IsAuthenticated => !string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiry;

    public AuthenticationService(HttpClient httpClient, ILogger<AuthenticationService> logger,
        ISecureTokenStorage tokenStorage, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _tokenStorage = tokenStorage;
        _configuration = configuration;

        try
        {
            _clientId = configuration["Auth:ClientId"]
                        ?? throw new ConfigurationException("Google 0Auth ClientId in not found in application configuration.",
                            "Authentication service is not properly configured. Please contact support.",
                            "Auth:ClientId");
            
            _redirectUri = configuration["Authentication:Google:RedirectUri"] 
                           ?? "http://localhost:8080/callback";
        }
        catch (Exception e) when (e is not ConfigurationException)
        {
            throw new ConfigurationException(
                "Failed to initialize Google authentication configuration.",
                "Authentication service is not properly configured. Please contact support.",
                "Auth",
                e);
        }

        _ = Task.Run(async () =>
        {
            try
            {
                await LoadStoredTokensAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to load stored tokens during startup.");
            }
        });
    }
    
    public async Task<bool> LoginAsync()
    {
        await _authSemaphore.WaitAsync();
        try
        {
            _logger.LogInformation("Starting Google OAuth login flow.");

            var codeVerifier = GeneratedCodeVerifier();
            var codeChallenge = GenerateCodeChallenge(codeVerifier);
            var state = GenerateSecureState();

            var authUrl = BuildAuthorizationUrl(codeChallenge, state);

            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));

            string authCode;

            try
            {
                authCode = await GetAuthorizationCodeAsync(authUrl, state, cts.Token);
            }
            catch (OperationCanceledException)
            {
                throw new AuthenticationTimeoutException(
                    "OAuth login process timed out after 2 minutes.",
                    timeout: TimeSpan.FromMinutes(2));
            }

            if (string.IsNullOrEmpty(authCode))
            {
                throw new AuthorizationException(
                    "Authorization code not received from OAuth provider.",
                    "Sign-in was not completed. Please try again.");
            }

            var tokenResponse = await ExchangeCodeForTokensAsync(authCode, codeVerifier);
            await StoreTokensAsync(tokenResponse);
            SetupTokenRefresh();

            _logger.LogInformation("Google OAuth login completed successfully.");
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
            _logger.LogWarning("OAuth authorization failed");
            throw;
        }
        catch (TokenStorageException)
        {
            _logger.LogError("Failed to store authentication tokens");
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error during OAuth login.");
            throw new AuthenticationException(
                $"Unexpected error during login: {e.Message}",
                "An unexpected error occurred during sign-in. Please try again.",
                innerException: e);
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

            if (!string.IsNullOrEmpty(_accessToken))
            {
                try
                {
                    await RevokeTokenAsync(_accessToken);
                }
                catch (NetworkException e)
                {
                    _logger.LogWarning(e, "Failed to revoke access token due to network error.");
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, "Failed to revoke access token.");
                }
            }

            try
            {
                await _tokenStorage.DeleteTokenAsync("google_access_token");
                await _tokenStorage.DeleteTokenAsync("google_refresh_token");
                await _tokenStorage.DeleteTokenAsync("google_token_expiry");
            }
            catch (Exception e)
            {
                throw new TokenStorageException(
                    $"Failed to delete stored tokens: {e.Message}",
                    "Unable to complete sign-out. Some authentication data may remain.",
                    operation: "delete",
                    innerException: e);
            }

            _accessToken = null;
            _refreshToken = null;
            _tokenExpiry = DateTime.MinValue;

            _refreshTimer?.Dispose();
            _refreshTimer = null;

            AuthenticationChanged?.Invoke(this, new AuthenticationChangedEventArgs(false));
        }
        finally
        {
            _authSemaphore.Release();
        }
    }

    public async Task<string?> GetValidAccessTokenAsync()
    {
        await _authSemaphore.WaitAsync();

        try
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                throw new AuthenticationException(
                    "No access token available.",
                    "You are not signed in. Please sign in to continue.");
            }

            if (DateTime.UtcNow.AddMinutes(2) < _tokenExpiry) return _accessToken;
            _logger.LogDebug("Access token expired or expiring soon. Refreshing.");

            try
            {
                await RefreshTokenAsync();
            }
            catch (TokenRefreshException)
            {
                await LogoutAsync();
                throw;
            }

            return _accessToken;
        }
        finally
        {
            _authSemaphore.Release();
        }
    }

    private async Task<string> GetAuthorizationCodeAsync(string authUrl, string expectedState,
        CancellationToken cancellationToken)
    {
        HttpListener? listener = null;

        try
        {
            var port = FindAvailablePort();
            var callbackUrl = $"http://localhost:{port}/callback";

            listener = new HttpListener();
            listener.Prefixes.Add($"http://localhost:{port}/");
            listener.Start();

            authUrl = authUrl.Replace(_redirectUri, callbackUrl);

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = authUrl,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception e)
            {
                throw new AuthorizationException(
                    "Failed to open browser for authentication.",
                    "Unable to open your browser for sign-in. Please try again.",
                    innerException: e);
            }

            var contextTask = listener.GetContextAsync();
            var context = await contextTask.WaitAsync(cancellationToken);

            await SendCallbackResponseAsync(context);

            var query = context.Request.Url?.Query;
            if (string.IsNullOrEmpty(query))
            {
                throw new AuthorizationException(
                    "No query parameters received in OAuth callback.",
                    "Invalid response from sign-in provider. Please try again.");
            }

            var queryParams = System.Web.HttpUtility.ParseQueryString(query);
            var code = queryParams["code"];
            var state = queryParams["state"];
            var error = queryParams["error"];
            var errorDescription = queryParams["error_description"];

            if (!string.IsNullOrEmpty(error))
            {
                throw new AuthorizationException(
                    $"OAuth provider returned error: {error}",
                    "Sign-in was denied or failed. Please try again.",
                    error,
                    errorDescription);
            }

            if (state != expectedState)
            {
                throw new OAuthStateException(
                    "OAuth state parameter mismatch - possible CSRF attack.",
                    expectedState,
                    receivedState: state);
            }

            if (string.IsNullOrEmpty(code))
            {
                throw new AuthenticationException(
                    "Authorization code not received from OAuth provider.",
                    "Sign-in was not completed. Please try again.");
            }

            return code;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e) when (e is AuthorizationException or OAuthStateException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new AuthorizationException(
                $"Unexpected error during authorization: {e.Message}",
                "An error occurred during sign-in. Please try again.",
                innerException: e);
        }
        finally
        {
            listener?.Stop();
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
                                    // Auto-close after 3 seconds
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
            context.Response.Close();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send callback response");
        }
    }

    private async Task<TokenResponse> ExchangeCodeForTokensAsync(string code, string codeVerifier)
    {
        try
        {
            const string tokenEndpoint = "https://oauth2.googleapis.com/token";
            var parameters = new Dictionary<string, string>
            {
                ["client_id"] = _clientId,
                ["code"] = code,
                ["grant_type"] = "authorization_code",
                ["redirect_uri"] = _redirectUri,
                ["code_verifier"] = codeVerifier
            };

            var content = new FormUrlEncodedContent(parameters);

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.PostAsync(tokenEndpoint, content);
            }
            catch (HttpRequestException e)
            {
                throw new NetworkException(
                    $"Network error during token exchange: {e.Message}",
                    endpoint: tokenEndpoint,
                    innerException: e);
            }
            catch (TaskCanceledException e)
            {
                throw new NetworkException(
                    "Token exchange request timed out",
                    endpoint: tokenEndpoint,
                    timeout: _httpClient.Timeout,
                    innerException: e);
            }

            using (response)
            {
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new AuthorizationException(
                        $"Token exchange failed with status {response.StatusCode}",
                        "Failed to complete sign-in. PLease try again.",
                        error: response.StatusCode.ToString(),
                        errorDescription: json);
                }

                try
                {
                    var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(json, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                    });

                    if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
                    {
                        throw new AuthorizationException(
                            "Invalid taken response received from OAuth provider.",
                            "Sign-in failed due to invalid response. Please try again.");
                    }

                    return tokenResponse;
                }
                catch (JsonException e)
                {
                    throw new AuthorizationException(
                        $"Failed to parse token response: {e.Message}",
                        "Sign-in failed due to invalid response format.Please try again.",
                        innerException: e);
                }
            }
        }
        catch (Exception e) when (e is NetworkException or AuthorizationException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new AuthorizationException(
                $"Unexpected error during token exchange: {e.Message}",
                "An unexpected error occurred during sign-in. Please try again.",
                innerException: e);
        }
    }

    private async Task<bool> RefreshTokenAsync()
    {
        if (string.IsNullOrEmpty(_refreshToken))
            return false;

        try
        {
            const string tokenEndpoint = "https://oauth2.googleapis.com/token";
            var parameters = new Dictionary<string, string>
            {
                ["client_id"] = _clientId,
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = _refreshToken
            };

            var content = new FormUrlEncodedContent(parameters);
            using var response = await _httpClient.PostAsync(tokenEndpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Token refresh failed: {StatusCode} - {Response}", response.StatusCode, errorContent);
                return false;
            }

            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            });

            if (tokenResponse == null)
                return false;
            
            await StoreTokensAsync(tokenResponse);
            SetupTokenRefresh();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error refreshing token.");
            return false;
        }
    }

    private async Task RevokeTokenAsync(string token)
    {
        try
        {
            var revokeEndpoint = $"https://oauth2.googleapis.com/revoke?token={token}";
            using var response = await _httpClient.PostAsync(revokeEndpoint, null);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Token revocation returned: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error revoking token.");
        }
    }

    private async Task StoreTokensAsync(TokenResponse tokenResponse)
    {
        _accessToken = tokenResponse.AccessToken;

        if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
        {
            _refreshToken = tokenResponse.RefreshToken;
            await _tokenStorage.StoreTokenAsync("google_refresh_token", _refreshToken);
        }
        
        _tokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);

        await _tokenStorage.StoreTokenAsync("google_access_token", _accessToken);
        await _tokenStorage.StoreTokenAsync("google_token_expiry", _tokenExpiry.ToString("0"));
    }

    private async Task LoadStoredTokensAsync()
    {
        try
        {
            _accessToken = await _tokenStorage.RetrieveTokenAsync("google_access_token");
            _refreshToken = await _tokenStorage.RetrieveTokenAsync("google_refresh_token");

            var expiryString = await _tokenStorage.RetrieveTokenAsync("google_token_expiry");
            if (DateTime.TryParse(expiryString, out var expiry))
            {
                _tokenExpiry = expiry;
            }

            if (IsAuthenticated)
            {
                SetupTokenRefresh();
                AuthenticationChanged?.Invoke(this, new AuthenticationChangedEventArgs(true));
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error loading stored tokens.");
        }
    }

    private void SetupTokenRefresh()
    {
        _refreshTimer?.Dispose();

        var refreshTime = _tokenExpiry.AddMinutes(-5);
        var delay = refreshTime - DateTime.UtcNow;

        if (delay > TimeSpan.Zero)
        {
            _refreshTimer = new Timer(async _ => await RefreshTokenAsync(), null, delay, Timeout.InfiniteTimeSpan);
        }
    }
    
    private static string GeneratedCodeVerifier()
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

    private string BuildAuthorizationUrl(string codeChallenge, string state)
    {
        var scopes = Uri.EscapeDataString("openid email profile");
        return $"https://accounts.google.com/o/oauth2/v2/auth?" +
               $"client_id={Uri.EscapeDataString(_clientId)}&" +
               $"redirect_uri={Uri.EscapeDataString(_redirectUri)}&" +
               $"response_type=code&" +
               $"scope={scopes}&" +
               $"code_challenge={codeChallenge}&" +
               $"code_challenge_method=S256&" +
               $"state={state}&" +
               $"access_type=offline&" +
               $"prompt=consent";
    }

    private static int FindAvailablePort()
    {
        using var socket = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 0);
        socket.Start();
        var port = ((IPEndPoint)socket.LocalEndpoint).Port;
        socket.Stop();
        return port;
    }

    public void Dispose()
    {
        _refreshTimer?.Dispose();
        _authSemaphore?.Dispose();
    }

    private class TokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; } = string.Empty;
        public string? Scope { get; set; }
    }
}