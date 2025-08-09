using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.Utils.Interfaces;

public interface IRepository
{
    Task<IEnumerable<T>> GetAllAsync<T>();
}