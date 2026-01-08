using Microsoft.AspNetCore.Mvc;
using ProductMVC.Contexts;
using ProductMVC.Models;
using ProductMVC.ViewModels.TagViewModels;

namespace ProductMVC.Areas.Admin.Controllers;
[Area("Admin")]
[AutoValidateAntiforgeryToken]
public class TagController : Controller
{
    private readonly ProniaDbContext _context;

    public TagController(ProniaDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var tags = _context.Tags.Select(x => new TagGetVM()
        {
            Id = x.Id,
            Name = x.Name,
        }).ToList();

        return View(tags);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(TagCreateVM vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        Tag newTag = new Tag()
        {
            Name = vm.Name,
        };

        _context.Tags.Add(newTag);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var tag = _context.Tags.Find(id);
        if (tag == null)
        {
            return NotFound();
        }
        _context.Tags.Remove(tag);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Update(int id)
    {
        var tag = _context.Tags.Find(id);
        if (tag == null)
        {
            return NotFound();
        }

        TagUpdateVM vm = new TagUpdateVM()
        {
            Id = tag.Id,
            Name = tag.Name,
        };

        return View(vm);
    }

    [HttpPost]
    public IActionResult Update(TagUpdateVM vm)
    {
        if (!ModelState.IsValid) { return View(vm); }
        var existTag = _context.Tags.Find(vm.Id);
        if (existTag == null)
        {
            return NotFound();
        }
        existTag.Name = vm.Name;
        _context.Tags.Update(existTag);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}
