using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VintagePosuda.Web.Models;

namespace VintagePosuda.Web.Data.Configurations;
public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Items");
        builder.HasKey(x => x.Id);

        // Скалярные свойства.
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        // Индексы для часто используемых полей поиска.
        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.Year);
        builder.HasIndex(x => x.Status);

        // N→1 на завод-изготовитель.
        builder.HasOne(x => x.Manufacturer)
            .WithMany(m => m.Items)
            .HasForeignKey(x => x.ManufacturerId)
            .OnDelete(DeleteBehavior.Restrict);

        // N→1 на категорию.
        builder.HasOne(x => x.Category)
            .WithMany(c => c.Items)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // N→1 на материал.
        builder.HasOne(x => x.Material)
            .WithMany(m => m.Items)
            .HasForeignKey(x => x.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        // 1↔1 c ItemDetails: PK у ItemDetails совпадает с FK.
        builder.HasOne(x => x.Details)
            .WithOne(d => d.Item)
            .HasForeignKey<ItemDetails>(d => d.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // 1→N с фотографиями.
        builder.HasMany(x => x.Photos)
            .WithOne(p => p.Item)
            .HasForeignKey(p => p.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
