using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductMVC.Models;

namespace ProductMVC.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.Property(x=>x.ImageUrl).IsRequired().HasMaxLength(512);
        builder.HasOne(x=>x.Product).WithMany(x=>x.ProductImages).HasForeignKey(x=>x.ProductId).HasPrincipalKey(x=>x.Id).OnDelete(DeleteBehavior.Cascade);
    }
}
