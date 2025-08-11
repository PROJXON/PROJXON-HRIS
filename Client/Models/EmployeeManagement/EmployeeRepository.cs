using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Client.Utils.Classes;
using Client.Utils.Interfaces;
using Microsoft.Extensions.Logging;
using Shared.EmployeeManagement.Responses;

namespace Client.Models.EmployeeManagement;

public class EmployeeRepository(IApiClient apiClient, ILogger<EmployeeRepository> logger)
    : BaseRepository<EmployeeResponse>(apiClient, logger), IEmployeeRepository
{
    protected override string EntityEndpoint => "api/Employee";

    public async Task<Result<IEnumerable<EmployeeResponse>>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    public async Task<Result<IEnumerable<EmployeeResponse>>> GetActiveEmployeesAsync(CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    public async Task<Result<IEnumerable<EmployeeResponse>>> SearchEmployeesAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }
}