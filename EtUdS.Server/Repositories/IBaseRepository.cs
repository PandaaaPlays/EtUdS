namespace EtUdS.Server.Repositories;

public interface IBaseRepository
{
    Task Save();
    Task<TEntity> GetById<TEntity>(int id) where TEntity : class;
    Task<IEnumerable<TEntity>> GetAll<TEntity>() where TEntity : class;
    Task<TEntity> Add<TEntity>(TEntity entity) where TEntity : class;
    void Update<TEntity>(TEntity entity) where TEntity : class;
    void Delete<TEntity>(TEntity entity) where TEntity : class;
}