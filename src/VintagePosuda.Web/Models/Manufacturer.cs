namespace VintagePosuda.Web.Models;
public class Manufacturer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Country { get; set; }
    public int? FoundedYear { get; set; }
    public int? ClosedYear { get; set; }
    public ICollection<Item> Items { get; set; } = new List<Item>();
}
