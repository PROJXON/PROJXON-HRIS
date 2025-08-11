using System.Threading;
using System.Threading.Tasks;
using Client.Utils.Classes;

namespace Client.Utils.Interfaces;

public interface IApiClient
{
    Task<ApiResponse<T>> GetAllAsync<T>(string endpoint, CancellationToken cancellationToken = default);
    Task<ApiResponse<T>> GetByIdAsync<T>(string endpoint, int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default);
    Task<ApiResponse<T>> PutAsync<T>(string endpoint, int id, object data, CancellationToken cancellationToken = default);
    Task<ApiResponse<object?>> DeleteAsync<T>(string endpoint, CancellationToken cancellationToken = default);
}