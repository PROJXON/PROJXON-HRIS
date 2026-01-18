using CloudSync.Modules.UserManagement.Models;
using Shared.UserManagement.Requests;

namespace CloudSync.Modules.UserManagement.Repositories.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByGoogleUserIdAsync(string googleUserId);
    
    /// <summary>
    /// Creates a new User from an invitation, linking to an Employee and setting the Role.
    /// </summary>
    /// <param name="invitedUser">The invitation record.</param>
    /// <param name="googleUserId">The Google OAuth subject ID.</param>
    /// <param name="employeeId">The Employee ID to link to.</param>
    /// <param name="roleId">The Role ID to assign (default: 3 for Intern).</param>
    /// <returns>The newly created User.</returns>
    Task<User> CreateUserFromInvitationAsync(InvitedUser invitedUser, string googleUserId, int employeeId, int roleId);
    
    /// <summary>
    /// Legacy overload for backward compatibility (without employee/role linking).
    /// </summary>
    Task<User> CreateUserFromInvitationAsync(InvitedUser invitedUser, string googleUserId);
    
    Task UpdateAsync(int id, UpdateUserRequest request);
    Task UpdateLastLoginTimeAsync(int id);
    Task DeleteAsync(int id);
}
