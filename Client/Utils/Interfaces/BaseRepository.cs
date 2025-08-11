using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Client.Utils.Classes;
using Microsoft.Extensions.Logging;

namespace Client.Utils.Interfaces;

public abstract class BaseRepository<TEntity, TKey>(IApiClient apiClient, ILogger logger) : IRepository<TEntity, TKey>
    where TEntity : class
{
    protected readonly IApiClient _apiClient = apiClient;
    protected readonly ILogger _logger = logger;
    protected abstract string EntityEndpoint { get; }


    public async Task<IEnumerable<TEntity>> GetAllAsync<T>()
    {
        throw new System.NotImplementedException();
    }

    public async Task<Result<TEntity>> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    public async Task<Result<TEntity>> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    public async Task<Result<TEntity>> UpdateAsync(TKey id, TEntity entity, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    public async Task<Result> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }
}