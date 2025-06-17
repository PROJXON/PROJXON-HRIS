namespace Shared.EmployeeManagement.BaseDtos;

public class AddressBase
{
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? Country { get; set; }
    public string? StateOrProvince { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? TimeZone { get; set; }
}