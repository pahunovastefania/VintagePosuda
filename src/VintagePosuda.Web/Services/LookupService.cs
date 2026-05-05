using Microsoft.EntityFrameworkCore;
using VintagePosuda.Web.Data;
using VintagePosuda.Web.Models;

namespace VintagePosuda.Web.Services;

public abstract class LookupService<T> where T : class, INamedEntity
{
    protected readonly ApplicationDbContext Db;
    private readonly DbSet<T> _set;

    protected LookupService(ApplicationDbContext db)
    {
        Db = db;
        _set = db.Set<T>();
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
        => await _set.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);

    public Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        => _set.FindAsync(new object[] { id }, ct).AsTask();

    public async Task<int> CreateAsync(T entity, CancellationToken ct = default)
    {
        await _set.AddAsync(entity, ct);
        await Db.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _set.Update(entity);
        await Db.SaveChangesAsync(ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _set.FindAsync(new object[] { id }, ct);
        if (entity is null)
        {
            return false;
        }

        _set.Remove(entity);
        await Db.SaveChangesAsync(ct);
        return true;
    }
}
