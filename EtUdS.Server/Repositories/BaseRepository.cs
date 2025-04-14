using etuds.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace EtUdS.Server.Repositories;

public class BaseRepository : IBaseRepository
{
    protected readonly AppDbContext _context;

    public BaseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }
    
    public async Task<TEntity> GetById<TEntity>(int id) where TEntity : class
    {
        return await _context.Set<TEntity>().FindAsync(id);
    }

    public async Task<IEnumerable<TEntity>> GetAll<TEntity>() where TEntity : class
    {
        return await _context.Set<TEntity>().ToListAsync();
    }
    
    public async Task<TEntity> Add<TEntity>(TEntity entity) where TEntity : class
    {
        var result = await _context.Set<TEntity>().AddAsync(entity);
        return result.Entity;
    }
    
    public void Update<TEntity>(TEntity entity) where TEntity : class
    {
        _context.Set<TEntity>().Update(entity);
    }

    public void Delete<TEntity>(TEntity entity) where TEntity : class
    {
        _context.Set<TEntity>().Remove(entity);
    }
}