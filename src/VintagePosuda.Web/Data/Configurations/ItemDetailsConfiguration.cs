using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VintagePosuda.Web.Models;

namespace VintagePosuda.Web.Data.Configurations;
public class ItemDetailsConfiguration : IEntityTypeConfiguration<ItemDetails>
{
    public void Configure(EntityTypeBuilder<ItemDetails> builder)
    {
        builder.ToTable("ItemDetails");
        builder.HasKey(x => x.ItemId);

        builder.Property(x => x.Origin).HasMaxLength(200);
        builder.Property(x => x.Provenance).HasMaxLength(2000);
        builder.Property(x => x.Condition).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Defects).HasMaxLength(1000);
    }
}
