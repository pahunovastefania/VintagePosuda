namespace VintagePosuda.Web.Repositories;

/// <summary>
/// Common CRUD operations for EF Core entities.
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
public interface IRepository<T>
    where T : class
{
    /// <summary>Returns all entities.</summary>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);

    /// <summary>Finds an entity by its primary key.</summary>
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>Adds a new entity and saves changes.</summary>
    Task AddAsync(T entity, CancellationToken ct = default);

    /// <summary>Updates an entity and saves changes.</summary>
    Task UpdateAsync(T entity, CancellationToken ct = default);

    /// <summary>Deletes an entity by id. Returns false when the entity is not found.</summary>
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
