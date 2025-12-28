using Microsoft.AspNetCore.Mvc;
using ProductMVC.Contexts;
using ProductMVC.ViewModels.ProductViewModels;

namespace ProductMVC.Controllers;

public class ShopController : Controller
{
    private readonly ProniaDbContext _context;

    public ShopController(ProniaDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var products=_context.Products.Select(p=>new ProductGetVM()
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Rating = p.Rating,
            CategoryName = p.Category.Name,
            MainImageUrl = p.MainImageUrl,
            HoverImageUrl = p.HoverImageUrl,
            SKU = p.SKU,
        }).ToList();

        return View(products);
    }
}
