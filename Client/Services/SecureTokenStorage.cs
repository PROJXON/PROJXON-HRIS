using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Client.Services;

/// <summary>
/// Development implementation of secure token storage.
/// Stores encrypted tokens in a local file.
/// For production, consider using platform-specific secure storage (Windows Credential Manager, macOS Keychain, etc.)
/// </summary>
public class SecureTokenStorage : ISecureTokenStorage
{
    private readonly ILogger<SecureTokenStorage> _logger;
    private readonly string _storagePath;
    private readonly byte[] _entropy;
    private Dictionary<string, string> _cache = new();
    private readonly SemaphoreSlim _lock = new(1, 1);

    public SecureTokenStorage(ILogger<SecureTokenStorage> logger)
    {
        _logger = logger;
        
        // Store in user's app data folder
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(appDataPath, "PROJXON-HRIS");
        
        if (!Directory.Exists(appFolder))
        {
            Directory.CreateDirectory(appFolder);
        }
        
        _storagePath = Path.Combine(appFolder, "tokens.enc");
        
        // Generate or load entropy for encryption
        var entropyPath = Path.Combine(appFolder, ".entropy");
        if (File.Exists(entropyPath))
        {
            _entropy = File.ReadAllBytes(entropyPath);
        }
        else
        {
            _entropy = RandomNumberGenerator.GetBytes(16);
            File.WriteAllBytes(entropyPath, _entropy);
            // Set file as hidden on Windows
            if (OperatingSystem.IsWindows())
            {
                File.SetAttributes(entropyPath, FileAttributes.Hidden);
            }
        }
        
        // Load existing tokens
        LoadTokens();
    }

    public async Task StoreTokenAsync(string key, string value)
    {
        await _lock.WaitAsync();
        try
        {
            _cache[key] = value;
            await SaveTokensAsync();
            _logger.LogDebug("Token stored: {Key}", key);
        }
        finally
        {
            _lock.Release();
        }
    }

    public Task<string?> RetrieveTokenAsync(string key)
    {
        _cache.TryGetValue(key, out var value);
        return Task.FromResult(value);
    }

    public async Task DeleteTokenAsync(string key)
    {
        await _lock.WaitAsync();
        try
        {
            if (_cache.Remove(key))
            {
                await SaveTokensAsync();
                _logger.LogDebug("Token deleted: {Key}", key);
            }
        }
        finally
        {
            _lock.Release();
        }
    }

    public Task<bool> TokenExistsAsync(string key)
    {
        return Task.FromResult(_cache.ContainsKey(key));
    }

    private void LoadTokens()
    {
        try
        {
            if (!File.Exists(_storagePath))
            {
                _cache = new Dictionary<string, string>();
                return;
            }

            var encryptedData = File.ReadAllBytes(_storagePath);
            var decryptedData = DecryptData(encryptedData);
            var json = Encoding.UTF8.GetString(decryptedData);
            _cache = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load tokens, starting fresh");
            _cache = new Dictionary<string, string>();
        }
    }

    private async Task SaveTokensAsync()
    {
        try
        {
            var json = JsonSerializer.Serialize(_cache);
            var data = Encoding.UTF8.GetBytes(json);
            var encryptedData = EncryptData(data);
            await File.WriteAllBytesAsync(_storagePath, encryptedData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save tokens");
            throw;
        }
    }

    private byte[] EncryptData(byte[] data)
    {
        using var aes = Aes.Create();
        aes.Key = DeriveKey(_entropy);
        aes.GenerateIV();
        
        using var encryptor = aes.CreateEncryptor();
        var encrypted = encryptor.TransformFinalBlock(data, 0, data.Length);
        
        // Prepend IV to encrypted data
        var result = new byte[aes.IV.Length + encrypted.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
        Buffer.BlockCopy(encrypted, 0, result, aes.IV.Length, encrypted.Length);
        
        return result;
    }

    private byte[] DecryptData(byte[] encryptedData)
    {
        using var aes = Aes.Create();
        aes.Key = DeriveKey(_entropy);
        
        // Extract IV from the beginning
        var iv = new byte[aes.BlockSize / 8];
        Buffer.BlockCopy(encryptedData, 0, iv, 0, iv.Length);
        aes.IV = iv;
        
        // Extract encrypted content
        var cipherText = new byte[encryptedData.Length - iv.Length];
        Buffer.BlockCopy(encryptedData, iv.Length, cipherText, 0, cipherText.Length);
        
        using var decryptor = aes.CreateDecryptor();
        return decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
    }

    private static byte[] DeriveKey(byte[] entropy)
    {
        // Simple key derivation - in production, use a proper KDF
        using var sha256 = SHA256.Create();
        var machineId = Environment.MachineName + Environment.UserName;
        var combined = new byte[entropy.Length + Encoding.UTF8.GetByteCount(machineId)];
        Buffer.BlockCopy(entropy, 0, combined, 0, entropy.Length);
        Encoding.UTF8.GetBytes(machineId, 0, machineId.Length, combined, entropy.Length);
        return sha256.ComputeHash(combined);
    }
}
