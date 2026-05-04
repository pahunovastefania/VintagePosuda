using VintagePosuda.Web.Models;

namespace VintagePosuda.Web.Services;

/// <summary>
/// CRUD operations for catalog categories.
/// </summary>
public interface ICategoryService
{
    /// <summary>Returns all categories.</summary>
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct = default);

    /// <summary>Finds a category by id.</summary>
    Task<Category?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>Creates a category and returns its id.</summary>
    Task<int> CreateAsync(Category category, CancellationToken ct = default);

    /// <summary>Updates a category.</summary>
    Task UpdateAsync(Category category, CancellationToken ct = default);

    /// <summary>Deletes a category by id.</summary>
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
