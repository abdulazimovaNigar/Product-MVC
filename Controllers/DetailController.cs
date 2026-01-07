using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductMVC.Contexts;

namespace ProductMVC.Controllers;

public class DetailController : Controller
{
    private readonly ProniaDbContext _context;

    public DetailController(ProniaDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(int id)
    {
        var product = _context.Products.Include(x => x.Category).Include(x => x.ProductImages).Include(x => x.ProductTags).ThenInclude(x=>x.Tag).FirstOrDefault(x => x.Id == id);
        return View(product);
    }
}
