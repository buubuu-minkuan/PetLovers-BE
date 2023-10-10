namespace Data.Repositories.GenericRepository
{
    public interface IRepository<T> where T : class
    {
        Task<T?> Get(Guid id);
        Task<Guid> Insert(T entity);

    }
}


