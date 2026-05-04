using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VintagePosuda.Web.Models;

namespace VintagePosuda.Web.Data.Configurations;
public class ManufacturerConfiguration : IEntityTypeConfiguration<Manufacturer>
{
    public void Configure(EntityTypeBuilder<Manufacturer> builder)
    {
        builder.ToTable("Manufacturers");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Country)
            .HasMaxLength(100);

        // Имя завода уникально — два «ЛФЗ» в одной БД не нужны.
        builder.HasIndex(x => x.Name).IsUnique();
    }
}
