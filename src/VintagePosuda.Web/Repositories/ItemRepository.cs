using Microsoft.EntityFrameworkCore;
using VintagePosuda.Web.Data;
using VintagePosuda.Web.Models;
using VintagePosuda.Web.Services;

namespace VintagePosuda.Web.Repositories;
public class ItemRepository : Repository<Item>, IItemRepository
{
    public ItemRepository(ApplicationDbContext db)
        : base(db)
    {
    }
    public async Task<IReadOnlyList<Item>> SearchAsync(SearchQuery query, CancellationToken ct = default)
    {
        IQueryable<Item> q = Set
            .AsNoTracking()
            .Include(x => x.Manufacturer)
            .Include(x => x.Category)
            .Include(x => x.Material);

        if (!string.IsNullOrWhiteSpace(query.NameContains))
        {
            string pattern = $"%{query.NameContains.Trim()}%";
            q = q.Where(x => EF.Functions.Like(x.Name, pattern));
        }

        if (!string.IsNullOrWhiteSpace(query.ManufacturerContains))
        {
            string pattern = $"%{query.ManufacturerContains.Trim()}%";
            q = q.Where(x => x.Manufacturer != null && EF.Functions.Like(x.Manufacturer.Name, pattern));
        }

        if (query.Year is not null)
        {
            q = q.Where(x => x.Year == query.Year);
        }

        if (query.Status is not null)
        {
            q = q.Where(x => x.Status == query.Status);
        }

        if (query.ManufacturerId is not null)
        {
            q = q.Where(x => x.ManufacturerId == query.ManufacturerId);
        }

        if (query.CategoryId is not null)
        {
            q = q.Where(x => x.CategoryId == query.CategoryId);
        }

        return await q.OrderByDescending(x => x.CreatedAt).ToListAsync(ct);
    }
    public async Task<Item?> GetWithDetailsAsync(int id, CancellationToken ct = default)
    {
        return await Set
            .AsNoTracking()
            .Include(x => x.Manufacturer)
            .Include(x => x.Category)
            .Include(x => x.Material)
            .Include(x => x.Details)
            .Include(x => x.Photos)
            .Include(x => x.ItemTags)!
                .ThenInclude(it => it.Tag)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }
    public async Task<IReadOnlyList<Item>> GetByStatusAsync(ItemStatus status, CancellationToken ct = default)
    {
        return await Set
            .AsNoTracking()
            .Include(x => x.Manufacturer)
            .Include(x => x.Category)
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);
    }
    public async Task SetTagsAsync(int itemId, IEnumerable<int> tagIds, CancellationToken ct = default)
    {
        var current = await Db.ItemTags.Where(x => x.ItemId == itemId).ToListAsync(ct);
        Db.ItemTags.RemoveRange(current);

        foreach (int tagId in tagIds.Distinct())
        {
            Db.ItemTags.Add(new ItemTag { ItemId = itemId, TagId = tagId });
        }

        await Db.SaveChangesAsync(ct);
    }
}
