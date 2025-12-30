namespace Client.Utils.Enums;

/// <summary>
/// Defines the types of ViewModels that can be navigated to in the application
/// </summary>
public enum ViewModelType
{
    Login,
    PortalSelection,
    HRDashboard,
    InternDashboard,
    Dashboard, // Legacy - can be removed after migration
    EmployeesList, // Legacy - replaced by Employees
    EmployeeDetails,
    Profile,
    Employees // New employees list view with cards
}
