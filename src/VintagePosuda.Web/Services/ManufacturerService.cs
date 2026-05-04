using Microsoft.EntityFrameworkCore;
using VintagePosuda.Web.Data;
using VintagePosuda.Web.Models;

namespace VintagePosuda.Web.Services;
public class ManufacturerService : IManufacturerService
{
    private readonly ApplicationDbContext _db;
    public ManufacturerService(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<IReadOnlyList<Manufacturer>> GetAllAsync(CancellationToken ct = default)
        => await _db.Manufacturers.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);
    public Task<Manufacturer?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Manufacturers.FindAsync(new object[] { id }, ct).AsTask();
    public async Task<int> CreateAsync(Manufacturer manufacturer, CancellationToken ct = default)
    {
        await _db.Manufacturers.AddAsync(manufacturer, ct);
        await _db.SaveChangesAsync(ct);
        return manufacturer.Id;
    }
    public async Task UpdateAsync(Manufacturer manufacturer, CancellationToken ct = default)
    {
        _db.Manufacturers.Update(manufacturer);
        await _db.SaveChangesAsync(ct);
    }
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.Manufacturers.FindAsync(new object[] { id }, ct);
        if (entity is null)
        {
            return false;
        }

        _db.Manufacturers.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
