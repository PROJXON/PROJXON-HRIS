using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client.Utils.Classes;
using Microsoft.Extensions.Logging;

namespace Client.Utils.Interfaces;

public abstract class BaseRepository<TEntity>(IApiClient apiClient, ILogger logger) : IRepository<TEntity>
    where TEntity : class
{
    protected readonly IApiClient ApiClient = apiClient;
    protected readonly ILogger _logger = logger;
    protected abstract string EntityEndpoint { get; }


    public async Task<Result<IEnumerable<TEntity>>> GetAllAsync<T>(CancellationToken cancellationToken = default)
    {
        var response = await ApiClient.GetAllAsync<IEnumerable<TEntity>>(EntityEndpoint, cancellationToken);
        return response.IsSuccess
            ? Result<IEnumerable<TEntity>>.Success(response.Data ?? [])
            : Result<IEnumerable<TEntity>>.Failure(response.ErrorMessage);
    }

    public async Task<Result<TEntity>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await ApiClient.GetByIdAsync<TEntity>(EntityEndpoint, id, cancellationToken);
        return response is { IsSuccess: true, Data: not null }
            ? Result<TEntity>.Success(response.Data)
            : Result<TEntity>.Failure(response.ErrorMessage);
    }

    public async Task<Result<TEntity>> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var response = await ApiClient.PostAsync<TEntity>(EntityEndpoint, entity, cancellationToken);
        return response is { IsSuccess: true, Data: not null }
            ? Result<TEntity>.Success(response.Data)
            : Result<TEntity>.Failure(response.ErrorMessage);
    }

    public async Task<Result<TEntity>> UpdateAsync(int id, TEntity entity, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }
}