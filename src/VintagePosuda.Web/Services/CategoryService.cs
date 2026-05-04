using Microsoft.EntityFrameworkCore;
using VintagePosuda.Web.Data;
using VintagePosuda.Web.Models;

namespace VintagePosuda.Web.Services;
public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _db;
    public CategoryService(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct = default)
        => await _db.Categories.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);
    public Task<Category?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Categories.FindAsync(new object[] { id }, ct).AsTask();
    public async Task<int> CreateAsync(Category category, CancellationToken ct = default)
    {
        await _db.Categories.AddAsync(category, ct);
        await _db.SaveChangesAsync(ct);
        return category.Id;
    }
    public async Task UpdateAsync(Category category, CancellationToken ct = default)
    {
        _db.Categories.Update(category);
        await _db.SaveChangesAsync(ct);
    }
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.Categories.FindAsync(new object[] { id }, ct);
        if (entity is null)
        {
            return false;
        }

        _db.Categories.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
