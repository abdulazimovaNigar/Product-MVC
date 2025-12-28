using Microsoft.AspNetCore.Mvc;
using ProductMVC.Contexts;

namespace ProductMVC.Controllers;

public class HomeController : Controller
{
    private readonly ProniaDbContext _context;

    public HomeController(ProniaDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {

        return View();
    }
}
