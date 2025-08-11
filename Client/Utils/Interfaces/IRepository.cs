using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.Utils.Interfaces;

public interface IRepository<TEntity, TKey> where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync<T>();
}