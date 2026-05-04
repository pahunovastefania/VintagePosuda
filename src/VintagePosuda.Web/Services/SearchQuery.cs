using VintagePosuda.Web.Models;

namespace VintagePosuda.Web.Services;
public class SearchQuery
{
    public string? NameContains { get; set; }
    public string? ManufacturerContains { get; set; }
    public int? Year { get; set; }
    public ItemStatus? Status { get; set; }
    public int? ManufacturerId { get; set; }
    public int? CategoryId { get; set; }
}
