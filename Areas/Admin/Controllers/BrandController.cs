using Microsoft.AspNetCore.Mvc;
using ProductMVC.Contexts;
using ProductMVC.Models;
using ProductMVC.ViewModels.BrandViewModel;

namespace ProductMVC.Areas.Admin.Controllers;
[Area("Admin")]
[AutoValidateAntiforgeryToken]
public class BrandController : Controller
{
    private readonly ProniaDbContext _context;

    public BrandController(ProniaDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var brands = _context.Brands.Select(x => new BrandGetVM()
        {
            Id = x.Id,
            Name = x.Name,
        }).ToList();

        return View(brands);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(BrandCreateVM vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        Brand newBrand = new Brand()
        {
            Name = vm.Name
        };

        _context.Brands.Add(newBrand);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var brand = _context.Brands.Find(id);
        if (brand == null)
        {
            return NotFound();
        }
        _context.Brands.Remove(brand);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Update(int id)
    {
        var brand = _context.Brands.Find(id);
        if (brand == null)
        {
            return NotFound();
        }

        BrandUpdateVM vm = new BrandUpdateVM()
        {
            Id = brand.Id,
            Name = brand.Name,
        };

        return View(vm);
    }

    [HttpPost]
    public IActionResult Update(BrandUpdateVM vm)
    {
        if (!ModelState.IsValid) { return View(vm); }
        var existBrand = _context.Brands.Find(vm.Id);
        if (existBrand == null)
        {
            return NotFound();
        }
        existBrand.Name = vm.Name;
        _context.Brands.Update(existBrand);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}
