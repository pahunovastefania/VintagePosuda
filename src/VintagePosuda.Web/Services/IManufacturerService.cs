using VintagePosuda.Web.Models;

namespace VintagePosuda.Web.Services;

/// <summary>
/// CRUD operations for manufacturers.
/// </summary>
public interface IManufacturerService
{
    /// <summary>Returns all manufacturers.</summary>
    Task<IReadOnlyList<Manufacturer>> GetAllAsync(CancellationToken ct = default);

    /// <summary>Finds a manufacturer by id.</summary>
    Task<Manufacturer?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>Creates a manufacturer and returns its id.</summary>
    Task<int> CreateAsync(Manufacturer manufacturer, CancellationToken ct = default);

    /// <summary>Updates a manufacturer.</summary>
    Task UpdateAsync(Manufacturer manufacturer, CancellationToken ct = default);

    /// <summary>Deletes a manufacturer by id.</summary>
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
