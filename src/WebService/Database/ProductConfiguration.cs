using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebService.Model;

namespace WebService.Database;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable(
            "product",
            null,
            tableBuilder => { tableBuilder.HasCheckConstraint("price_check", "price > 0"); }
        );
        builder.Property(p => p.Code).HasMaxLength(50);
        builder.Property(p => p.Description).HasMaxLength(200);
        builder.Property(p => p.Price).HasColumnType("numeric(20,3)");
    }
}