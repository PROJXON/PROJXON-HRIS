using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.Utils.Interfaces;

public interface IApiClient
{
    Task<IEnumerable<T>> FetchAllAsync<T>();
}