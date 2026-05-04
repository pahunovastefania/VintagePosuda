using VintagePosuda.Web.Models;
using VintagePosuda.Web.Repositories;

namespace VintagePosuda.Web.Services;
public class ReportService : IReportService
{
    private readonly IItemRepository _items;
    public ReportService(IItemRepository items)
    {
        _items = items;
    }
    public async Task<StockReport> GetInStockReportAsync(CancellationToken ct = default)
    {
        var items = await _items.GetByStatusAsync(ItemStatus.InStock, ct);
        return new StockReport { Items = items };
    }
    public async Task<StockReport> GetSoldReportAsync(CancellationToken ct = default)
    {
        var items = await _items.GetByStatusAsync(ItemStatus.Sold, ct);
        return new StockReport { Items = items };
    }
}
