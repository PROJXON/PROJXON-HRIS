using Shared.EmployeeManagement.BaseDtos;

namespace Shared.EmployeeManagement.Responses;

public class EmployeeContactInfoResponse : EmployeeContactInfoBase
{
    public AddressResponse? Address { get; set; }
}