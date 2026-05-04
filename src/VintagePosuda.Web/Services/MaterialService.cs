using Microsoft.EntityFrameworkCore;
using VintagePosuda.Web.Data;
using VintagePosuda.Web.Models;

namespace VintagePosuda.Web.Services;
public class MaterialService : IMaterialService
{
    private readonly ApplicationDbContext _db;
    public MaterialService(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<IReadOnlyList<Material>> GetAllAsync(CancellationToken ct = default)
        => await _db.Materials.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);
    public Task<Material?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Materials.FindAsync(new object[] { id }, ct).AsTask();
    public async Task<int> CreateAsync(Material material, CancellationToken ct = default)
    {
        await _db.Materials.AddAsync(material, ct);
        await _db.SaveChangesAsync(ct);
        return material.Id;
    }
    public async Task UpdateAsync(Material material, CancellationToken ct = default)
    {
        _db.Materials.Update(material);
        await _db.SaveChangesAsync(ct);
    }
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.Materials.FindAsync(new object[] { id }, ct);
        if (entity is null)
        {
            return false;
        }

        _db.Materials.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
