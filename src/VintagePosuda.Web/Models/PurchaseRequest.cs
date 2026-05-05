namespace VintagePosuda.Web.Models;

public class PurchaseRequest
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public Item? Item { get; set; }

    public string BuyerName { get; set; } = string.Empty;
    public string BuyerPhone { get; set; } = string.Empty;
    public string? BuyerEmail { get; set; }
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public PurchaseRequestStatus Status { get; set; } = PurchaseRequestStatus.New;
}
