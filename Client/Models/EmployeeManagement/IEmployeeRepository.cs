using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Client.Utils.Classes;
using Client.Utils.Interfaces;
using Shared.EmployeeManagement.Responses;

namespace Client.Models.EmployeeManagement;

public interface IEmployeeRepository : IRepository<EmployeeResponse>
{

}