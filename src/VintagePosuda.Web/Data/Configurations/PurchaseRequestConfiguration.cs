using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VintagePosuda.Web.Models;

namespace VintagePosuda.Web.Data.Configurations;

public class PurchaseRequestConfiguration : IEntityTypeConfiguration<PurchaseRequest>
{
    public void Configure(EntityTypeBuilder<PurchaseRequest> builder)
    {
        builder.ToTable("PurchaseRequests");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.BuyerName)
            .IsRequired()
            .HasMaxLength(80);

        builder.Property(x => x.BuyerPhone)
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(x => x.BuyerEmail)
            .HasMaxLength(120);

        builder.Property(x => x.Comment)
            .HasMaxLength(500);

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CreatedAt);

        builder.HasOne(x => x.Item)
            .WithMany()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
