using VintagePosuda.Web.Models;
using VintagePosuda.Web.Services;

namespace VintagePosuda.Web.Repositories;

/// <summary>
/// Catalog item repository with search and eager-loading operations.
/// </summary>
public interface IItemRepository : IRepository<Item>
{
    /// <summary>Searches catalog items by the provided filters.</summary>
    Task<IReadOnlyList<Item>> SearchAsync(SearchQuery query, CancellationToken ct = default);

    /// <summary>Returns an item with details, photos, tags and reference data.</summary>
    Task<Item?> GetWithDetailsAsync(int id, CancellationToken ct = default);

    /// <summary>Returns items with the selected status.</summary>
    Task<IReadOnlyList<Item>> GetByStatusAsync(ItemStatus status, CancellationToken ct = default);

    /// <summary>Replaces item tag links with the provided set of tag ids.</summary>
    Task SetTagsAsync(int itemId, IEnumerable<int> tagIds, CancellationToken ct = default);
}
