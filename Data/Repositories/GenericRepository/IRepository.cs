using System.Linq.Expressions;

namespace Data.Repositories.GenericRepository
{
    public interface IRepository<T> where T : class
    {
        Task<T?> Get(Guid id);
        Task<Guid> Insert(T entity);
        Task<bool> Update(T entity);
        Task<bool> Remove(T entity);
    }
}


