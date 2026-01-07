using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductMVC.Models;

namespace ProductMVC.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(x=>x.Name).IsRequired().HasMaxLength(256);
        builder.Property(x=>x.Description).IsRequired(false).HasMaxLength(256);
        builder.Property(x=>x.SKU).IsRequired(false).HasMaxLength(256);
        builder.HasIndex(x=>x.SKU).IsUnique();
        builder.Property(x=>x.Price).IsRequired().HasPrecision(10,2);
        builder.Property(x => x.Rating).IsRequired();
        builder.Property(x=>x.MainImageUrl).IsRequired().HasMaxLength(512);
        builder.Property(x=>x.HoverImageUrl).IsRequired().HasMaxLength(512);

        builder.ToTable(opt =>
        {
            opt.HasCheckConstraint("CK_Products_Price", "[Price]>0");
        });

        builder.ToTable(opt =>
        {
            opt.HasCheckConstraint("CK_Products_Rating", "[Rating] between 0 and 5");
        });


        builder.HasOne(x => x.Category).WithMany(x => x.Products).HasForeignKey(x=>x.CategoryId).HasPrincipalKey(x=>x.Id).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Brand).WithMany(x => x.Products).HasForeignKey(x => x.BrandId).HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x=>x.ProductTags).WithOne(x=>x.Product).HasForeignKey(x=>x.ProductId).HasPrincipalKey(x=>x.Id).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.ProductImages).WithOne(x => x.Product).HasForeignKey(x => x.ProductId).HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Cascade);
    }
}
