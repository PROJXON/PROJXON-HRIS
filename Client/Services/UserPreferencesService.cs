using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Client.Utils.Enums;
using Microsoft.Extensions.Logging;

namespace Client.Services;

/// <summary>
/// Manages user preferences stored in local application data
/// </summary>
public class UserPreferencesService : IUserPreferencesService
{
    private readonly ILogger<UserPreferencesService> _logger;
    private readonly string _preferencesFilePath;
    private UserPreferences? _cachedPreferences;

    public UserPreferencesService(ILogger<UserPreferencesService> logger)
    {
        _logger = logger;
        
        // Store preferences in user's local app data
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(appDataPath, "ProjxonHRIS");
        
        // Ensure directory exists
        Directory.CreateDirectory(appFolder);
        
        _preferencesFilePath = Path.Combine(appFolder, "user_preferences.json");
    }

    public async Task<PortalType?> GetPortalPreferenceAsync()
    {
        try
        {
            var preferences = await LoadPreferencesAsync();
            return preferences?.PortalType;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading portal preference");
            return null;
        }
    }

    public async Task SetPortalPreferenceAsync(PortalType portalType)
    {
        try
        {
            var preferences = await LoadPreferencesAsync() ?? new UserPreferences();
            preferences.PortalType = portalType;
            preferences.LastUpdated = DateTime.UtcNow;
            
            await SavePreferencesAsync(preferences);
            _cachedPreferences = preferences;
            
            _logger.LogInformation("Portal preference saved: {PortalType}", portalType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving portal preference");
            throw;
        }
    }

    public async Task ClearPortalPreferenceAsync()
    {
        try
        {
            if (File.Exists(_preferencesFilePath))
            {
                File.Delete(_preferencesFilePath);
                _cachedPreferences = null;
                _logger.LogInformation("Portal preference cleared");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing portal preference");
            throw;
        }
        
        await Task.CompletedTask;
    }

    private async Task<UserPreferences?> LoadPreferencesAsync()
    {
        if (_cachedPreferences != null)
        {
            return _cachedPreferences;
        }

        if (!File.Exists(_preferencesFilePath))
        {
            return null;
        }

        try
        {
            var json = await File.ReadAllTextAsync(_preferencesFilePath);
            _cachedPreferences = JsonSerializer.Deserialize<UserPreferences>(json);
            return _cachedPreferences;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading preferences file");
            return null;
        }
    }

    private async Task SavePreferencesAsync(UserPreferences preferences)
    {
        var json = JsonSerializer.Serialize(preferences, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
        
        await File.WriteAllTextAsync(_preferencesFilePath, json);
    }

    /// <summary>
    /// Internal class to store user preferences
    /// </summary>
    private class UserPreferences
    {
        public PortalType? PortalType { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
