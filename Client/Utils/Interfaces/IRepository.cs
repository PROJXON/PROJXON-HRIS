using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Client.Utils.Classes;

namespace Client.Utils.Interfaces;

public interface IRepository<TEntity, TKey> where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync<T>();
    Task<Result<TEntity>> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<Result<TEntity>> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<Result<TEntity>> UpdateAsync(TKey id, TEntity entity, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(TKey id, CancellationToken cancellationToken = default);
}