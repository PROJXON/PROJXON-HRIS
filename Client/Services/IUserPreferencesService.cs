using System.Threading.Tasks;
using Client.Utils.Enums;

namespace Client.Services;

/// <summary>
/// Service for managing user preferences and settings
/// </summary>
public interface IUserPreferencesService
{
    /// <summary>
    /// Gets the user's saved portal preference
    /// </summary>
    /// <returns>The saved portal type, or null if no preference is saved</returns>
    Task<PortalType?> GetPortalPreferenceAsync();
    
    /// <summary>
    /// Saves the user's portal preference
    /// </summary>
    /// <param name="portalType">The portal type to save</param>
    Task SetPortalPreferenceAsync(PortalType portalType);
    
    /// <summary>
    /// Clears the user's saved portal preference
    /// </summary>
    Task ClearPortalPreferenceAsync();
}
