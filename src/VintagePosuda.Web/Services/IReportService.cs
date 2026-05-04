namespace VintagePosuda.Web.Services;

/// <summary>
/// Builds stock and sales reports.
/// </summary>
public interface IReportService
{
    /// <summary>Returns report for items currently in stock.</summary>
    Task<StockReport> GetInStockReportAsync(CancellationToken ct = default);

    /// <summary>Returns report for sold items.</summary>
    Task<StockReport> GetSoldReportAsync(CancellationToken ct = default);
}
