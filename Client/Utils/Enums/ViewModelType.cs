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
    EmployeesList,
    EmployeeDetails
}