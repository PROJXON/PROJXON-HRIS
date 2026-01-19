using Shared.EmployeeManagement.Responses;
using Shared.Responses.UserManagement;
using System;
using System.Threading.Tasks;


namespace Client.Services;

/// <summary>
/// Interface for session management service.
/// Stores and provides access to the currently logged-in user and employee data.
/// </summary>
public interface ISessionService
{
    /// <summary>
    /// The currently logged-in user's data.
    /// </summary>
    UserResponse? CurrentUser { get; }
    
    /// <summary>
    /// The currently logged-in user's employee profile data.
    /// </summary>
    EmployeeResponse? CurrentEmployee { get; }
    
    /// <summary>
    /// Indicates whether a user is currently logged in with valid session data.
    /// </summary>
    bool IsSessionValid { get; }
    
    /// <summary>
    /// The display name for the current user (preferred name or first name).
    /// </summary>
    string DisplayName { get; }
    
    /// <summary>
    /// The full name of the current user.
    /// </summary>
    string FullName { get; }
    
    /// <summary>
    /// The current user's profile picture URL.
    /// </summary>
    string? ProfilePictureUrl { get; }
    
    /// <summary>
    /// The current user's job title.
    /// </summary>
    string? JobTitle { get; }
    
    /// <summary>
    /// Event raised when session data is updated.
    /// </summary>
    event EventHandler<SessionChangedEventArgs>? SessionChanged;
    
    /// <summary>
    /// Initializes the session with user and employee data after login.
    /// </summary>
    Task InitializeSessionAsync(UserResponse user, EmployeeResponse? employee);
    
    /// <summary>
    /// Updates the current employee data (e.g., after profile edit).
    /// </summary>
    Task UpdateEmployeeAsync(EmployeeResponse employee);
    
    /// <summary>
    /// Clears all session data (on logout).
    /// </summary>
    Task ClearSessionAsync();
    
    /// <summary>
    /// Refreshes the employee data from the API.
    /// </summary>
    Task RefreshEmployeeDataAsync();
}

/// <summary>
/// Event arguments for session change events.
/// </summary>
public class SessionChangedEventArgs : EventArgs
{
    public SessionChangeType ChangeType { get; }
    public UserResponse? User { get; }
    public EmployeeResponse? Employee { get; }
    
    public SessionChangedEventArgs(SessionChangeType changeType, UserResponse? user, EmployeeResponse? employee)
    {
        ChangeType = changeType;
        User = user;
        Employee = employee;
    }
}

/// <summary>
/// Type of session change.
/// </summary>
public enum SessionChangeType
{
    Initialized,
    Updated,
    Cleared
}
