using VintagePosuda.Web.Models;

namespace VintagePosuda.Web.Services;

/// <summary>
/// CRUD operations for item tags.
/// </summary>
public interface ITagService
{
    /// <summary>Returns all tags.</summary>
    Task<IReadOnlyList<Tag>> GetAllAsync(CancellationToken ct = default);

    /// <summary>Finds a tag by id.</summary>
    Task<Tag?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>Creates a tag and returns its id.</summary>
    Task<int> CreateAsync(Tag tag, CancellationToken ct = default);

    /// <summary>Updates a tag.</summary>
    Task UpdateAsync(Tag tag, CancellationToken ct = default);

    /// <summary>Deletes a tag by id.</summary>
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
