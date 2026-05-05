using Microsoft.EntityFrameworkCore;
using VintagePosuda.Web.Data;
using VintagePosuda.Web.Models;

namespace VintagePosuda.Web.Services;

public class PurchaseRequestService : IPurchaseRequestService
{
    private readonly ApplicationDbContext _db;

    public PurchaseRequestService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<int> CreateAsync(PurchaseRequest request, CancellationToken ct = default)
    {
        request.CreatedAt = DateTime.UtcNow;
        request.Status = PurchaseRequestStatus.New;
        request.BuyerName = request.BuyerName.Trim();
        request.BuyerPhone = request.BuyerPhone.Trim();
        request.BuyerEmail = string.IsNullOrWhiteSpace(request.BuyerEmail) ? null : request.BuyerEmail.Trim();
        request.Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim();

        await _db.PurchaseRequests.AddAsync(request, ct);
        await _db.SaveChangesAsync(ct);
        return request.Id;
    }

    public async Task<IReadOnlyList<PurchaseRequest>> GetAllAsync(CancellationToken ct = default)
        => await _db.PurchaseRequests
            .AsNoTracking()
            .Include(x => x.Item)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

    public async Task<bool> SetStatusAsync(int id, PurchaseRequestStatus status, CancellationToken ct = default)
    {
        var request = await _db.PurchaseRequests.FindAsync(new object[] { id }, ct);
        if (request is null)
        {
            return false;
        }

        request.Status = status;
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
