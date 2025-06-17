namespace Shared.EmployeeManagement.BaseDtos;

public class EmployeeContactInfoBase
{
    public string? Phone { get; set; }
    public string? InternationalPhone { get; set; }
    public string? InternationalPhoneType { get; set; }
    public int? AddressId { get; set; }
    public string? ProjxonEmail { get; set; }
    public string? PersonalEmail { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
}