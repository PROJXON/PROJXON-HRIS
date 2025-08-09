using System.Threading.Tasks;
using Client.Utils.Classes;

namespace Client.Utils.Interfaces;

public interface IApiClient
{
    Task<ApiResponse<T>> FetchAllAsync<T>();
}