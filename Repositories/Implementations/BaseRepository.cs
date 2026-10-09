using Microsoft.EntityFrameworkCore;
using SwiftIn.Context;
using SwiftIn.Models.Entities;

namespace SwiftIn.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    protected readonly SwiftInContext _db;
    protected readonly DbSet<T> _set;

    public BaseRepository(SwiftInContext db)
    {
        _db = db;
        _set = db.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id) => await _set.FindAsync(id);

    public async Task<List<T>> GetAllAsync() => await _set.ToListAsync();

    public async Task AddAsync(T entity)
    {
        await _set.AddAsync(entity);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _set.Update(entity);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _set.Remove(entity);
        await _db.SaveChangesAsync();
    }
}