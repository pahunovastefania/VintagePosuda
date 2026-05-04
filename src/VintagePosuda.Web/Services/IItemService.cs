using VintagePosuda.Web.Models;

namespace VintagePosuda.Web.Services;

/// <summary>
/// Business operations for catalog items.
/// </summary>
public interface IItemService
{
    /// <summary>Searches catalog items by filters.</summary>
    Task<IReadOnlyList<Item>> SearchAsync(SearchQuery query, CancellationToken ct = default);

    /// <summary>Returns a catalog item by id.</summary>
    Task<Item?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>Creates a new item and returns its id.</summary>
    Task<int> CreateAsync(Item item, IEnumerable<int>? tagIds = null, CancellationToken ct = default);

    /// <summary>Updates editable item fields and tag links.</summary>
    Task UpdateAsync(Item item, IEnumerable<int>? tagIds = null, CancellationToken ct = default);

    /// <summary>Marks an item as sold.</summary>
    Task<bool> MarkAsSoldAsync(int id, DateTime? soldAt = null, CancellationToken ct = default);

    /// <summary>Moves an item to archive.</summary>
    Task<bool> ArchiveAsync(int id, CancellationToken ct = default);

    /// <summary>Restores an item to the catalog.</summary>
    Task<bool> RestoreAsync(int id, CancellationToken ct = default);

    /// <summary>Deletes an item by id.</summary>
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
