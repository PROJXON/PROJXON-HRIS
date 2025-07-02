using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Client.Utils.Exceptions.Auth;
using Microsoft.Extensions.Logging;

namespace Client.Services;

public class LinuxSecretServiceStorage : ISecureTokenStorage
{
    private readonly ILogger<LinuxSecretServiceStorage> _logger;
    private readonly string _applicationName;
    private readonly string _serviceName;

    public LinuxSecretServiceStorage(ILogger<LinuxSecretServiceStorage> logger, string applicationName)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _applicationName = applicationName ?? throw new ArgumentNullException(nameof(applicationName));
        _serviceName = $"com.{SanitizeServiceName(applicationName)}.tokens";
    }

    public async Task StoreTokenAsync(string key, string token)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be null or whitespace.", nameof(token));

        try
        {
            _logger.LogDebug("Storing token for key: {Key}", key);

            if (await IsSecretToolAvailableAsync())
            {
                await StoreWithSecretToolAsync(key, token);
                _logger.LogDebug("Successfully stored token using secret-tool");
                return;
            }
            
            _logger.LogWarning("secret-tool not available, falling back to encrypted file storage.");
            await StoreWithEncryptedFileAsync(key, token);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to store token for key: {Key}", key);
            throw new TokenStorageException(
                $"Failed to store secure token for key {key}",
                "Unable to securely store your authentication. PLease try again.",
                "Store",
                key,
                e);
        }
    }
    
     public async Task<string?> RetrieveTokenAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or whitespace", nameof(key));

            try
            {
                _logger.LogDebug("Retrieving token for key: {Key}", key);
                
                if (await IsSecretToolAvailableAsync())
                {
                    var token = await RetrieveWithSecretToolAsync(key);
                    if (token != null)
                    {
                        _logger.LogDebug("Successfully retrieved token using secret-tool");
                        return token;
                    }
                }
                
                _logger.LogDebug("Attempting to retrieve from encrypted file storage");
                return await RetrieveWithEncryptedFileAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve token for key: {Key}", key);
                throw new TokenStorageException(
                    $"Failed to retrieve secure token for key '{key}': {ex.Message}",
                    "Unable to retrieve your stored authentication. Please try logging in again.",
                    "Retrieve",
                    key,
                    ex);
            }
        }

        public async Task DeleteTokenAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or whitespace", nameof(key));

            try
            {
                _logger.LogDebug("Deleting token for key: {Key}", key);
                
                if (await IsSecretToolAvailableAsync())
                {
                    await DeleteWithSecretToolAsync(key);
                    _logger.LogDebug("Successfully deleted token using secret-tool");
                }
                
                await DeleteWithEncryptedFileAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete token for key: {Key}", key);
                throw new TokenStorageException(
                    $"Failed to delete secure token for key '{key}': {ex.Message}",
                    "Unable to remove your stored authentication. This may affect future logins.",
                    "Delete",
                    key,
                    ex);
            }
        }

        #region Secret Tool Implementation

        private async Task<bool> IsSecretToolAvailableAsync()
        {
            try
            {
                using var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "which",
                        Arguments = "secret-tool",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                await process.WaitForExitAsync();
                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        private async Task StoreWithSecretToolAsync(string key, string token)
        {
            var label = $"{_applicationName} - {key}";
            var service = _serviceName;

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "secret-tool",
                    Arguments = $"store --label=\"{label}\" service \"{service}\" key \"{key}\"",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.Start();

            await using var writer = process.StandardInput;
            await writer.WriteAsync(token);
            await writer.FlushAsync();
            writer.Close();

            await process.WaitForExitAsync();
            
            if (process.ExitCode != 0)
            {
                var error = await process.StandardError.ReadToEndAsync();
                throw new TokenStorageException(
                    $"secret-tool store operation failed with exit code {process.ExitCode}: {error}",
                    "Unable to securely store your authentication using the system keyring.",
                    "Store",
                    key);
            }
        }
        
        private async Task<string?> RetrieveWithSecretToolAsync(string key)
        {
            var service = _serviceName;

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "secret-tool",
                    Arguments = $"lookup service \"{service}\" key \"{key}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode == 0 && !string.IsNullOrWhiteSpace(output))
            {
                return output.Trim();
            }

            return null;
        }

        private async Task DeleteWithSecretToolAsync(string key)
        {
            var service = _serviceName;

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "secret-tool",
                    Arguments = $"clear service \"{service}\" key \"{key}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            await process.WaitForExitAsync();

            // secret-tool clear returns 1 if nothing was found, which is not necessarily an error
            if (process.ExitCode != 0 && process.ExitCode != 1)
            {
                var error = await process.StandardError.ReadToEndAsync();
                _logger.LogWarning("secret-tool clear returned non-zero exit code: {ExitCode}, Error: {Error}", 
                    process.ExitCode, error);
            }
        }
        
        #endregion

        #region Encrypted File Fallback

        private readonly string _storageDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".local", "share");

        private string GetTokenFilePath(string key)
        {
            var safeFileName = Convert.ToBase64String(Encoding.UTF8.GetBytes(key))
                .Replace('/', '_').Replace('+', '-').TrimEnd('=');
            var appDirectory = Path.Combine(_storageDirectory, SanitizeServiceName(_applicationName));
            Directory.CreateDirectory(appDirectory);
            return Path.Combine(appDirectory, $"{safeFileName}.token");
        }

        private async Task StoreWithEncryptedFileAsync(string key, string token)
        {
            var filePath = GetTokenFilePath(key);
            var tokenData = new TokenData
            {
                Key = key,
                Token = token,
                CreatedAt = DateTimeOffset.UtcNow,
                Application = _applicationName
            };

            var json = JsonSerializer.Serialize(tokenData);
            var encrypted = ProtectData(json);
            
            await File.WriteAllBytesAsync(filePath, encrypted);
            
            // Set restrictive permissions (owner read/write only)
            if (File.Exists(filePath))
            {
                try
                {
                    await SetFilePermissionsAsync(filePath, "600");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to set restrictive permissions on token file");
                }
            }
        }

        private async Task<string?> RetrieveWithEncryptedFileAsync(string key)
        {
            var filePath = GetTokenFilePath(key);
            
            if (!File.Exists(filePath))
                return null;

            try
            {
                var encrypted = await File.ReadAllBytesAsync(filePath);
                var json = UnprotectData(encrypted);
                var tokenData = JsonSerializer.Deserialize<TokenData>(json);

                return tokenData?.Token;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to decrypt token file for key: {Key}", key);
                return null;
            }
        }

        private async Task DeleteWithEncryptedFileAsync(string key)
        {
            var filePath = GetTokenFilePath(key);
            
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete token file for key: {Key}", key);
                }
            }
            
            await Task.CompletedTask;
        }

        private async Task SetFilePermissionsAsync(string filePath, string permissions)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "chmod",
                    Arguments = $"{permissions} \"{filePath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            await process.WaitForExitAsync();
        }

        #endregion
        
        #region Data Protection (Simple XOR with user-specific key)

        private byte[] ProtectData(string data)
        {
            var dataBytes = Encoding.UTF8.GetBytes(data);
            var key = GetProtectionKey();
            var result = new byte[dataBytes.Length];

            for (int i = 0; i < dataBytes.Length; i++)
            {
                result[i] = (byte)(dataBytes[i] ^ key[i % key.Length]);
            }

            return result;
        }

        private string UnprotectData(byte[] encryptedData)
        {
            var key = GetProtectionKey();
            var result = new byte[encryptedData.Length];

            for (int i = 0; i < encryptedData.Length; i++)
            {
                result[i] = (byte)(encryptedData[i] ^ key[i % key.Length]);
            }

            return Encoding.UTF8.GetString(result);
        }

        private byte[] GetProtectionKey()
        {
            // Create a simple key based on user and machine information
            var keyData = $"{Environment.UserName}_{Environment.MachineName}_{_applicationName}";
            return Encoding.UTF8.GetBytes(keyData);
        }

        #endregion

        #region Helper Methods

        private static string SanitizeServiceName(string name)
        {
            return name.ToLowerInvariant()
                .Replace(" ", "")
                .Replace("-", "")
                .Replace("_", "");
        }

        private class TokenData
        {
            public string Key { get; set; } = string.Empty;
            public string Token { get; set; } = string.Empty;
            public DateTimeOffset CreatedAt { get; set; }
            public string Application { get; set; } = string.Empty;
        }

        #endregion
}