using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ProductMVC.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string? SKU { get; set; }
    public int Rating { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public string MainImageUrl { get; set; }
    public string HoverImageUrl { get; set; }
    public ICollection<ProductTag> ProductTags { get; set; } = [];
    public ICollection<ProductImage> ProductImages { get; set; } = [];
    public Brand Brand { get; set; }
    public int BrandId { get; set; }

}
