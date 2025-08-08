using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.EmployeeManagement.Responses;

namespace Client.Utils.Interfaces;

public interface IRepository
{
    Task<IEnumerable<T>> GetAllAsync<T>();
}