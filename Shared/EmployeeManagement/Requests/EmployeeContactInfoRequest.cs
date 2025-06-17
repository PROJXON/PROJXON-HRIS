using Shared.EmployeeManagement.BaseDtos;

namespace Shared.EmployeeManagement.Requests;

public class EmployeeContactInfoRequest : EmployeeContactInfoBase
{
    public AddressRequest? Address { get; set; }
}