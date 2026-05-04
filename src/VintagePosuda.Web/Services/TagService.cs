using Microsoft.EntityFrameworkCore;
using VintagePosuda.Web.Data;
using VintagePosuda.Web.Models;

namespace VintagePosuda.Web.Services;
public class TagService : ITagService
{
    private readonly ApplicationDbContext _db;
    public TagService(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<IReadOnlyList<Tag>> GetAllAsync(CancellationToken ct = default)
        => await _db.Tags.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);
    public Task<Tag?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Tags.FindAsync(new object[] { id }, ct).AsTask();
    public async Task<int> CreateAsync(Tag tag, CancellationToken ct = default)
    {
        await _db.Tags.AddAsync(tag, ct);
        await _db.SaveChangesAsync(ct);
        return tag.Id;
    }
    public async Task UpdateAsync(Tag tag, CancellationToken ct = default)
    {
        _db.Tags.Update(tag);
        await _db.SaveChangesAsync(ct);
    }
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.Tags.FindAsync(new object[] { id }, ct);
        if (entity is null)
        {
            return false;
        }

        _db.Tags.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
