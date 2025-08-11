using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Client.Utils.Classes;
using Client.Utils.Interfaces;
using Shared.EmployeeManagement.Responses;

namespace Client.Models.EmployeeManagement;

public interface IEmployeeRepository : IRepository<EmployeeResponse>
{
    Task<Result<IEnumerable<EmployeeResponse>>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<EmployeeResponse>>> GetActiveEmployeesAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<EmployeeResponse>>> SearchEmployeesAsync(string searchTerm, CancellationToken cancellationToken = default);
}