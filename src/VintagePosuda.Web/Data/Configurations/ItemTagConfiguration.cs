using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VintagePosuda.Web.Models;

namespace VintagePosuda.Web.Data.Configurations;
public class ItemTagConfiguration : IEntityTypeConfiguration<ItemTag>
{
    public void Configure(EntityTypeBuilder<ItemTag> builder)
    {
        builder.ToTable("ItemTags");

        // Составной первичный ключ.
        builder.HasKey(x => new { x.ItemId, x.TagId });

        builder.HasOne(x => x.Item)
            .WithMany(i => i.ItemTags)
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Tag)
            .WithMany(t => t.ItemTags)
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
