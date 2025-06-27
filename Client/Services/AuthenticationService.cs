using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.Converters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Client.Utils.Classes;
using Client.Utils.Enums;
using Client.Utils.Exceptions.ApplicationState;
using Client.Utils.Exceptions.Auth;
using Client.Utils.Exceptions.Network;
using Client.Utils.Interfaces;

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

            await sendCallbackResponseAsync(context);

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
}