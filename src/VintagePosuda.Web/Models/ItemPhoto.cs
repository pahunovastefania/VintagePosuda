namespace VintagePosuda.Web.Models;
public class ItemPhoto
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string Url { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public Item? Item { get; set; }
}
