using VintagePosuda.Web.Models;

namespace VintagePosuda.Web.Services;

public interface IPurchaseRequestService
{
    Task<int> CreateAsync(PurchaseRequest request, CancellationToken ct = default);

    Task<IReadOnlyList<PurchaseRequest>> GetAllAsync(CancellationToken ct = default);

    Task<bool> SetStatusAsync(int id, PurchaseRequestStatus status, CancellationToken ct = default);
}
