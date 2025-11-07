using Shared.EmployeeManagement.BaseDtos;

namespace Shared.EmployeeManagement.Requests;

public class EmployeeContactInfoRequest : EmployeeContactInfoBase
{
    public AddressRequest? PermanentAddress { get; set; }
    public AddressRequest? MailingAddress { get; set; }
}