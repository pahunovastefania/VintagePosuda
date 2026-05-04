namespace VintagePosuda.Web.Models;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? Year { get; set; }
    public decimal Price { get; set; }
    public ItemStatus Status { get; set; } = ItemStatus.InStock;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SoldAt { get; set; }

    public int ManufacturerId { get; set; }
    public int CategoryId { get; set; }
    public int MaterialId { get; set; }

    public Manufacturer? Manufacturer { get; set; }
    public Category? Category { get; set; }
    public Material? Material { get; set; }
    public ItemDetails? Details { get; set; }
    public ICollection<ItemPhoto> Photos { get; set; } = new List<ItemPhoto>();
    public ICollection<ItemTag> ItemTags { get; set; } = new List<ItemTag>();
}
