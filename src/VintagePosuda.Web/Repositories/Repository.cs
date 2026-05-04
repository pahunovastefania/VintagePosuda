using Microsoft.EntityFrameworkCore;
using VintagePosuda.Web.Data;

namespace VintagePosuda.Web.Repositories;

public class Repository<T> : IRepository<T>
    where T : class
{
    protected readonly ApplicationDbContext Db;

    public Repository(ApplicationDbContext db)
    {
        Db = db;
    }

    protected DbSet<T> Set => Db.Set<T>();

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
    {
        return await Set.AsNoTracking().ToListAsync(ct);
    }

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await Set.FindAsync(new object[] { id }, ct);
    }

    public virtual async Task AddAsync(T entity, CancellationToken ct = default)
    {
        await Set.AddAsync(entity, ct);
        await Db.SaveChangesAsync(ct);
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        Set.Update(entity);
        await Db.SaveChangesAsync(ct);
    }

    public virtual async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await Set.FindAsync(new object[] { id }, ct);
        if (entity is null)
        {
            return false;
        }

        Set.Remove(entity);
        await Db.SaveChangesAsync(ct);
        return true;
    }
}
