using Microsoft.EntityFrameworkCore;
using VintagePosuda.Web.Data;
using VintagePosuda.Web.Models;
using VintagePosuda.Web.Repositories;

namespace VintagePosuda.Web.Services;
public class ItemService : IItemService
{
    private readonly IItemRepository _items;
    private readonly ApplicationDbContext _db;
    public ItemService(IItemRepository items, ApplicationDbContext db)
    {
        _items = items;
        _db = db;
    }
    public Task<IReadOnlyList<Item>> SearchAsync(SearchQuery query, CancellationToken ct = default)
        => _items.SearchAsync(query, ct);
    public Task<Item?> GetByIdAsync(int id, CancellationToken ct = default)
        => _items.GetWithDetailsAsync(id, ct);
    public async Task<int> CreateAsync(Item item, IEnumerable<int>? tagIds = null, CancellationToken ct = default)
    {
        item.CreatedAt = DateTime.UtcNow;
        if (item.Status == ItemStatus.Sold && item.SoldAt is null)
        {
            item.SoldAt = DateTime.UtcNow;
        }

        await _db.Items.AddAsync(item, ct);
        await _db.SaveChangesAsync(ct);

        if (tagIds is not null)
        {
            await _items.SetTagsAsync(item.Id, tagIds, ct);
        }

        return item.Id;
    }
    public async Task UpdateAsync(Item item, IEnumerable<int>? tagIds = null, CancellationToken ct = default)
    {
        var tracked = await _db.Items
            .Include(x => x.Details)
            .FirstOrDefaultAsync(x => x.Id == item.Id, ct);

        if (tracked is null)
        {
            throw new InvalidOperationException($"Предмет с Id={item.Id} не найден.");
        }

        tracked.Name = item.Name;
        tracked.Year = item.Year;
        tracked.Price = item.Price;
        tracked.Description = item.Description;
        tracked.ManufacturerId = item.ManufacturerId;
        tracked.CategoryId = item.CategoryId;
        tracked.MaterialId = item.MaterialId;

        if (item.Details is not null)
        {
            if (tracked.Details is null)
            {
                tracked.Details = new ItemDetails { ItemId = tracked.Id };
            }
            tracked.Details.Origin = item.Details.Origin;
            tracked.Details.Provenance = item.Details.Provenance;
            tracked.Details.Condition = item.Details.Condition;
            tracked.Details.Defects = item.Details.Defects;
        }

        await _db.SaveChangesAsync(ct);

        if (tagIds is not null)
        {
            await _items.SetTagsAsync(item.Id, tagIds, ct);
        }
    }
    public async Task<bool> MarkAsSoldAsync(int id, DateTime? soldAt = null, CancellationToken ct = default)
    {
        var item = await _db.Items.FindAsync(new object[] { id }, ct);
        if (item is null)
        {
            return false;
        }

        item.Status = ItemStatus.Sold;
        item.SoldAt = soldAt ?? DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }
    public async Task<bool> ArchiveAsync(int id, CancellationToken ct = default)
    {
        var item = await _db.Items.FindAsync(new object[] { id }, ct);
        if (item is null)
        {
            return false;
        }

        item.Status = ItemStatus.Archived;
        await _db.SaveChangesAsync(ct);
        return true;
    }
    public async Task<bool> RestoreAsync(int id, CancellationToken ct = default)
    {
        var item = await _db.Items.FindAsync(new object[] { id }, ct);
        if (item is null)
        {
            return false;
        }

        item.Status = ItemStatus.InStock;
        item.SoldAt = null;
        await _db.SaveChangesAsync(ct);
        return true;
    }
    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        => _items.DeleteAsync(id, ct);
}
