using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductMVC.Contexts;
using ProductMVC.Helpers;
using ProductMVC.Models;
using ProductMVC.ViewModels.ProductViewModels;

namespace ProductMVC.Areas.Admin.Controllers;
[Area("Admin")]
[AutoValidateAntiforgeryToken]
public class ProductController : Controller
{
    private readonly ProniaDbContext _context;
    private readonly IWebHostEnvironment _environment;
    public ProductController(ProniaDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public IActionResult Index()
    {
        var vms = _context.Products.Include(x => x.Category).Select(x => new ProductGetVM()
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            Price = x.Price,
            Rating = x.Rating,
            CategoryName = x.Category.Name,
            MainImageUrl = x.MainImageUrl,
            HoverImageUrl = x.HoverImageUrl,
            SKU = x.SKU,
        }).ToList();

        return View(vms);
    }

    [HttpGet]
    public IActionResult Create()
    {
        SendCategoriesWithViewBag();
        return View();
    }

    [HttpPost]
    public IActionResult Create(ProductCreateVM vm)
    {
        if (!ModelState.IsValid)
        {
            SendCategoriesWithViewBag();
            SendCategoriesWithViewBag();
            return View(vm);
        }

        if (!vm.MainImage.CheckType("image"))
        {
            SendCategoriesWithViewBag();
            ModelState.AddModelError("MainImage", "Please add only image!");
            return View(vm);
        }

        if (!vm.MainImage.CheckSize(2))
        {
            SendCategoriesWithViewBag();
            ModelState.AddModelError("MainImage", "Maximum 2 mb!");
            return View(vm);
        }

        if (!vm.HoverImage.CheckType("image"))
        {
            SendCategoriesWithViewBag();
            ModelState.AddModelError("HoverImage", "Please add only image!");
            return View(vm);
        }

        if (!vm.HoverImage.CheckSize(2))
        {
            SendCategoriesWithViewBag();
            ModelState.AddModelError("HoverImage", "Maximum 2 mb!");
            return View(vm);
        }

        var isExistCategory = _context.Categories.Any(x => x.Id == vm.CategoryId);
        if (!isExistCategory)
        {
            SendCategoriesWithViewBag();
            ModelState.AddModelError("", "This category doesnot exist!");
            return View(vm);
        }

        string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images");

        string mainImageName = vm.MainImage.SaveFile(folderPath);
        string hoverImageName = vm.HoverImage.SaveFile(folderPath);

        Product newProduct = new Product()
        {
            Name = vm.Name,
            Description = vm.Description,
            SKU = vm.SKU,
            CategoryId = vm.CategoryId,
            Price = vm.Price,
            Rating = vm.Rating,
            MainImageUrl = mainImageName,
            HoverImageUrl = hoverImageName,
        };

        _context.Products.Add(newProduct);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));

    }

    public IActionResult Delete(int id)
    {
        var product = _context.Products.Find(id);
        if (product == null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);
        _context.SaveChanges();

        string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images");

        if (System.IO.File.Exists(Path.Combine(folderPath, product.MainImageUrl)))
        {
            System.IO.File.Delete(Path.Combine(folderPath, product.MainImageUrl));
        }

        if (System.IO.File.Exists(Path.Combine(folderPath, product.HoverImageUrl)))
        {
            System.IO.File.Delete(Path.Combine(folderPath, product.HoverImageUrl));
        }

        return RedirectToAction(nameof(Index));


    }

    [HttpGet]
    public IActionResult Update(int id)
    {
        var product = _context.Products.Find(id);

        if (product == null)
        {
            return NotFound();
        }
        SendCategoriesWithViewBag();
        ProductUpdateVM vm = new ProductUpdateVM()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            CategoryId = product.CategoryId,
            Price = product.Price,
            SKU = product.SKU,
        };
        return View(vm);
    }

    [HttpPost]
    public IActionResult Update(ProductUpdateVM vm)
    {
        if (!ModelState.IsValid)
        {
            SendCategoriesWithViewBag();
            return View(vm);
        }

        if (!vm.MainImage?.CheckType("image") ?? false)
        {
            SendCategoriesWithViewBag();
            ModelState.AddModelError("MainImage", "Please add only image!");
            return View(vm);
        }

        if (vm.MainImage?.CheckSize(2) ?? false)
        {
            SendCategoriesWithViewBag();
            ModelState.AddModelError("MainImage", "Maximum 2 mb!");
            return View(vm);
        }

        if (!vm.HoverImage?.CheckType("image") ?? false)
        {
            SendCategoriesWithViewBag();
            ModelState.AddModelError("HoverImage", "Please add only image!");
            return View(vm);
        }

        if (vm.HoverImage?.CheckSize(2) ?? false)
        {
            SendCategoriesWithViewBag();
            ModelState.AddModelError("HoverImage", "Maximum 2 mb!");
            return View(vm);
        }

        var existProduct = _context.Products.Find(vm.Id);

        if (existProduct is null)
        {
            return NotFound();
        }

        var isExistCategory = _context.Categories.Any(c => c.Id == vm.CategoryId);

        if (!isExistCategory)
        {
            SendCategoriesWithViewBag();
            ModelState.AddModelError("CategoryId", "Category not found!");
            return View(vm);
        }
        existProduct.Name = vm.Name;
        existProduct.Description = vm.Description;
        existProduct.Price = vm.Price;
        existProduct.SKU = vm.SKU;
        existProduct.CategoryId = vm.CategoryId;

        string folderPath = Path.Combine(_environment.WebRootPath, "assets", "images", "website-images");

        if (vm.MainImage != null)
        {
            string newMainImageName = vm.MainImage.SaveFile(folderPath);
            if (System.IO.File.Exists(Path.Combine(folderPath, existProduct.MainImageUrl)))
            {
                System.IO.File.Delete(Path.Combine(folderPath, existProduct.MainImageUrl));
            }
            existProduct.MainImageUrl = newMainImageName;
        }

        if (vm.HoverImage != null)
        {
            string newHoverImageName = vm.HoverImage.SaveFile(folderPath);
            if (System.IO.File.Exists(Path.Combine(folderPath, existProduct.HoverImageUrl)))
            {
                System.IO.File.Delete(Path.Combine(folderPath, existProduct.HoverImageUrl));
            }
            existProduct.HoverImageUrl = newHoverImageName;
        }

        _context.Update(existProduct);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));

    }



    private void SendCategoriesWithViewBag()
    {
        var categories = _context.Categories.ToList();
        ViewBag.Categories = categories;
    }
}
