using Shared.EmployeeManagement.BaseDtos;

namespace Shared.EmployeeManagement.Responses;

public class EmployeeContactInfoResponse : EmployeeContactInfoBase
{
    public AddressResponse? PermanentAddress { get; set; }
    public AddressResponse? MailingAddress { get; set; }
}