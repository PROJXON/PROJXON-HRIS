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
    TimeOff,
    TimeOffRequests,
    Dashboard, // Legacy - can be removed after migration
    EmployeesList, // Legacy - replaced by Employees
    EmployeeDetails,
    Profile,
    InternProfile,
    Employees, // New employees list view with cards
    Attendance, // Attendance calendar view
    Recruitment, // Recruitment kanban board
    Forms, // Forms/Surveys list view
    CreateSurvey // Create new survey view
}
