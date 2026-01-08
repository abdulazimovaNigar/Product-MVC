using Microsoft.AspNetCore.Mvc;
using ProductMVC.Contexts;
using ProductMVC.Models;
using ProductMVC.ViewModels.CategoryViewModels;

namespace ProductMVC.Areas.Admin.Controllers;
[Area("Admin")]
[AutoValidateAntiforgeryToken]
public class CategoryController : Controller
{
    private readonly ProniaDbContext _context;

    public CategoryController(ProniaDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var vms = _context.Categories.Select(x => new CategoryGetVM()
        {
            Id = x.Id,
            Name = x.Name,
        }).ToList();
        return View(vms);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(CategoryCreateVM vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        Category newCategory = new Category()
        {
            Name = vm.Name,
        };
        _context.Categories.Add(newCategory);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));

    }

    public IActionResult Delete(int id)
    {
        var category = _context.Categories.Find(id);
        if (category == null)
        {
            return NotFound();
        }
        _context.Categories.Remove(category);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Update(int id)
    {
        var category = _context.Categories.Find(id);
        if (category == null)
        {
            return NotFound();
        }

        CategoryUpdateVM vm = new CategoryUpdateVM()
        {
            Id = category.Id,
            Name = category.Name,
        };

        return View(vm);

    }

    [HttpPost]
    public IActionResult Update(CategoryUpdateVM vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var existCategory = _context.Categories.Find(vm.Id);
        if (existCategory == null)
        {
            return NotFound();
        }

        existCategory.Name = vm.Name;
        _context.Categories.Update(existCategory);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

}
