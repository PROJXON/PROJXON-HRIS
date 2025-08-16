using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Client.Utils.Classes;

namespace Client.Utils.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
    Task<Result<IEnumerable<TEntity>>> GetAllAsync<T>(CancellationToken cancellationToken = default);
    Task<Result<TEntity>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<TEntity>> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<Result<TEntity>> UpdateAsync(int id, TEntity entity, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
}