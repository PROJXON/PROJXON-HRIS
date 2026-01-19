using System;
using System.Threading;
using Client.Models.EmployeeManagement;
using Microsoft.Extensions.Logging;
using Shared.EmployeeManagement.Responses;
using Shared.Responses.UserManagement;
using System.Threading.Tasks;

namespace Client.Services;

/// <summary>
/// Singleton service for managing the current user's session data.
/// Stores user and employee information after successful authentication.
/// </summary>
public class SessionService : ISessionService
{
    private readonly ILogger<SessionService> _logger;
    private readonly IEmployeeRepository _employeeRepository;
    
    private UserResponse? _currentUser;
    private EmployeeResponse? _currentEmployee;
    private readonly SemaphoreSlim _sessionLock = new(1, 1);

    public UserResponse? CurrentUser => _currentUser;
    public EmployeeResponse? CurrentEmployee => _currentEmployee;
    
    public bool IsSessionValid => _currentUser != null;

    public string DisplayName
    {
        get
        {
            if (_currentEmployee?.BasicInfo == null)
                return _currentUser?.Email ?? "User";
            
            return !string.IsNullOrWhiteSpace(_currentEmployee.BasicInfo.PreferredName)
                ? _currentEmployee.BasicInfo.PreferredName
                : _currentEmployee.BasicInfo.FirstName ?? _currentUser?.Email ?? "User";
        }
    }

    public string FullName
    {
        get
        {
            if (_currentEmployee?.BasicInfo == null)
                return _currentUser?.Email ?? "User";
            
            var firstName = _currentEmployee.BasicInfo.FirstName ?? "";
            var lastName = _currentEmployee.BasicInfo.LastName ?? "";
            var fullName = $"{firstName} {lastName}".Trim();
            
            return !string.IsNullOrWhiteSpace(fullName) 
                ? fullName 
                : _currentUser?.Email ?? "User";
        }
    }

    public string? ProfilePictureUrl => _currentEmployee?.Documents?.ProfilePictureUrl;

    public string? JobTitle => _currentEmployee?.PositionDetails?.PositionName;

    public event EventHandler<SessionChangedEventArgs>? SessionChanged;

    public SessionService(
        ILogger<SessionService> logger,
        IEmployeeRepository employeeRepository)
    {
        _logger = logger;
        _employeeRepository = employeeRepository;
    }

    public async Task InitializeSessionAsync(UserResponse user, EmployeeResponse? employee)
    {
        await _sessionLock.WaitAsync();
        try
        {
            _currentUser = user;
            _currentEmployee = employee;
            
            _logger.LogInformation(
                "Session initialized for user {Email}, EmployeeId: {EmployeeId}",
                user.Email,
                employee?.Id ?? 0);
            
            OnSessionChanged(SessionChangeType.Initialized);
        }
        finally
        {
            _sessionLock.Release();
        }
    }

    public async Task UpdateEmployeeAsync(EmployeeResponse employee)
    {
        await _sessionLock.WaitAsync();
        try
        {
            _currentEmployee = employee;
            
            _logger.LogInformation(
                "Employee data updated in session. EmployeeId: {EmployeeId}",
                employee.Id);
            
            OnSessionChanged(SessionChangeType.Updated);
        }
        finally
        {
            _sessionLock.Release();
        }
    }

    public async Task ClearSessionAsync()
    {
        await _sessionLock.WaitAsync();
        try
        {
            var email = _currentUser?.Email;
            
            _currentUser = null;
            _currentEmployee = null;
            
            _logger.LogInformation("Session cleared for user {Email}", email);
            
            OnSessionChanged(SessionChangeType.Cleared);
        }
        finally
        {
            _sessionLock.Release();
        }
    }

    public async Task RefreshEmployeeDataAsync()
    {
        if (_currentUser?.EmployeeId == null || _currentUser.EmployeeId == 0)
        {
            _logger.LogWarning("Cannot refresh employee data: No employee ID in session");
            return;
        }

        await _sessionLock.WaitAsync();
        try
        {
            var response = await _employeeRepository.GetByIdAsync(_currentUser.EmployeeId);
            
            // Fix: Changed .Data to .Value to match Result<T> pattern
            if (response.IsSuccess && response.Value != null)
            {
                _currentEmployee = response.Value;
                _logger.LogInformation("Employee data refreshed from API");
                OnSessionChanged(SessionChangeType.Updated);
            }
            else
            {
                _logger.LogWarning(
                    "Failed to refresh employee data: {Error}",
                    response.ErrorMessage);
            }
        }
        finally
        {
            _sessionLock.Release();
        }
    }

    private void OnSessionChanged(SessionChangeType changeType)
    {
        SessionChanged?.Invoke(this, new SessionChangedEventArgs(
            changeType,
            _currentUser,
            _currentEmployee));
    }
}