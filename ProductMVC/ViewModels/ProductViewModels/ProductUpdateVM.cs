using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ProductMVC.ViewModels.ProductViewModels;

public class ProductUpdateVM
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
    public IFormFile? MainImage { get; set; }
    public IFormFile? HoverImage { get; set; }
}
