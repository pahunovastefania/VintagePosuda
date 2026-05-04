using VintagePosuda.Web.Models;

namespace VintagePosuda.Web.Services;

/// <summary>
/// CRUD operations for materials.
/// </summary>
public interface IMaterialService
{
    /// <summary>Returns all materials.</summary>
    Task<IReadOnlyList<Material>> GetAllAsync(CancellationToken ct = default);

    /// <summary>Finds a material by id.</summary>
    Task<Material?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>Creates a material and returns its id.</summary>
    Task<int> CreateAsync(Material material, CancellationToken ct = default);

    /// <summary>Updates a material.</summary>
    Task UpdateAsync(Material material, CancellationToken ct = default);

    /// <summary>Deletes a material by id.</summary>
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
