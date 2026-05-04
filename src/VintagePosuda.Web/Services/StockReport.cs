using VintagePosuda.Web.Models;

namespace VintagePosuda.Web.Services;
public class StockReport
{
    public IReadOnlyList<Item> Items { get; init; } = Array.Empty<Item>();
    public int TotalCount => Items.Count;
    public decimal TotalPrice => Items.Sum(x => x.Price);
}
