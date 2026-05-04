namespace VintagePosuda.Web.Models;
public class ItemDetails
{
    public int ItemId { get; set; }
    public string? Origin { get; set; }
    public string? Provenance { get; set; }
    public string Condition { get; set; } = "Good";
    public string? Defects { get; set; }
    public Item? Item { get; set; }
}
