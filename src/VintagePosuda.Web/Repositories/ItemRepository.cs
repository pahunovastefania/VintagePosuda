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

        var items = await q.OrderByDescending(x => x.CreatedAt).ToListAsync(ct);

        if (!string.IsNullOrWhiteSpace(query.NameContains))
        {
            string[] terms = SplitSearchTerms(query.NameContains);
            items = items
                .Where(x => ContainsAllTerms(x.Name, terms))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(query.ManufacturerContains))
        {
            string[] terms = SplitSearchTerms(query.ManufacturerContains);
            items = items
                .Where(x => x.Manufacturer is not null && ContainsAllTerms(x.Manufacturer.Name, terms))
                .ToList();
        }

        return items;
    }

    private static string[] SplitSearchTerms(string value)
        => value
            .Trim()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    private static bool ContainsAllTerms(string value, IEnumerable<string> terms)
        => terms.All(term => value.Contains(term, StringComparison.CurrentCultureIgnoreCase));

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
