using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ProductMVC.Models;

public class Product
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    [Precision(10, 2)]
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string? SKU { get; set; }
    public int Rating { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    [Required]
    public string MainImageUrl { get; set; }
    [Required]
    public string HoverImageUrl { get; set; }
}
