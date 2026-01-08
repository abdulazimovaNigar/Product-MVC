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
        var vms = _context.Products.Include(x => x.Category).Include(x=>x.Brand).Select(x => new ProductGetVM()
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            Price = x.Price,
            Rating = x.Rating,
            CategoryName = x.Category.Name,
            BrandName = x.Brand.Name,
            MainImageUrl = x.MainImageUrl,
            HoverImageUrl = x.HoverImageUrl,
            SKU = x.SKU,
        }).ToList();

        return View(vms);
    }

    [HttpGet]
    public IActionResult Create()
    {
        SendItemsWithViewBag();
        return View();
    }

    [HttpPost]
    public IActionResult Create(ProductCreateVM vm)
    {
        if (!ModelState.IsValid)
        {
            SendItemsWithViewBag();
            SendItemsWithViewBag();
            return View(vm);
        }

        if (!vm.MainImage.CheckType("image"))
        {
            SendItemsWithViewBag();
            ModelState.AddModelError("MainImage", "Please add only image!");
            return View(vm);
        }

        if (!vm.MainImage.CheckSize(2))
        {
            SendItemsWithViewBag();
            ModelState.AddModelError("MainImage", "Maximum 2 mb!");
            return View(vm);
        }

        if (!vm.HoverImage.CheckType("image"))
        {
            SendItemsWithViewBag();
            ModelState.AddModelError("HoverImage", "Please add only image!");
            return View(vm);
        }

        if (!vm.HoverImage.CheckSize(2))
        {
            SendItemsWithViewBag();
            ModelState.AddModelError("HoverImage", "Maximum 2 mb!");
            return View(vm);
        }

        foreach (var image in vm.Images)
        {
            if (!image.CheckType("image"))
            {
                SendItemsWithViewBag();
                ModelState.AddModelError("Images", "Please add only image!");
                return View(vm);
            }

            if (!image.CheckSize(2))
            {
                SendItemsWithViewBag();
                ModelState.AddModelError("Images", "Maximum 2 mb!");
                return View(vm);
            }
        }

        var isExistCategory = _context.Categories.Any(x => x.Id == vm.CategoryId);
        if (!isExistCategory)
        {
            SendItemsWithViewBag();
            ModelState.AddModelError("", "This category doesnot exist!");
            return View(vm);
        }

        var isExistBrand=_context.Brands.Any(x=>x.Id==vm.BrandId);
        if (!isExistBrand)
        {
            SendItemsWithViewBag();
            ModelState.AddModelError("", "This brand doesnot exist!");
            return View(vm);
        }

        foreach (var tagId in vm.TagIds)
        {
            var isExistTag = _context.Tags.Any(x => x.Id == tagId);
            if (!isExistTag)
            {
                SendItemsWithViewBag();
                ModelState.AddModelError("", "This tag doesnot exist!");
                return View(vm);
            }
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
            BrandId = vm.BrandId,
            Price = vm.Price,
            Rating = vm.Rating,
            MainImageUrl = mainImageName,
            HoverImageUrl = hoverImageName,
            ProductTags = [],
            ProductImages = [],
        };

        foreach (var image in vm.Images)
        {
            string imageName = image.SaveFile(folderPath);

            ProductImage productImage = new ProductImage()
            {
                ImageUrl=imageName,
                Product=newProduct,
            };
            newProduct.ProductImages.Add(productImage);
        }

        foreach (var tagId in vm.TagIds)
        {
            ProductTag productTag = new ProductTag()
            {
                TagId = tagId,
                Product = newProduct
            };
            newProduct.ProductTags.Add(productTag);
        }

        _context.Products.Add(newProduct);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));

    }

    public IActionResult Delete(int id)
    {
        var product = _context.Products.Include(x=>x.ProductImages).FirstOrDefault(x=>x.Id==id);
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

        foreach (var image in product.ProductImages)
        {
            if (System.IO.File.Exists(Path.Combine(folderPath, image.ImageUrl)))
            {
                System.IO.File.Delete(Path.Combine(folderPath, image.ImageUrl));
            }
        }


        return RedirectToAction(nameof(Index));


    }

    [HttpGet]
    public IActionResult Update(int id)
    {
        var product = _context.Products.Include(x=>x.ProductTags).Include(x=>x.ProductImages).FirstOrDefault(x=>x.Id==id);

        if (product == null)
        {
            return NotFound();
        }
        SendItemsWithViewBag();
        ProductUpdateVM vm = new ProductUpdateVM()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            CategoryId = product.CategoryId,
            Price = product.Price,
            SKU = product.SKU,
            Rating = product.Rating,
            TagIds=product.ProductTags.Select(t => t.TagId).ToList(),
            HoverImagePath = product.HoverImageUrl,
            MainImagePath = product.MainImageUrl,
            ImageUrls=product.ProductImages.Select(x => x.ImageUrl).ToList(),
            ImageIds=product.ProductImages.Select(x=>x.Id).ToList(),
        };
        return View(vm);
    }

    [HttpPost]
    public IActionResult Update(ProductUpdateVM vm)
    {
        if (!ModelState.IsValid)
        {
            SendItemsWithViewBag();
            return View(vm);
        }

        if (!vm.MainImage?.CheckType("image") ?? false)
        {
            SendItemsWithViewBag();
            ModelState.AddModelError("MainImage", "Please add only image!");
            return View(vm);
        }

        if (vm.MainImage?.CheckSize(2) ?? false)
        {
            SendItemsWithViewBag();
            ModelState.AddModelError("MainImage", "Maximum 2 mb!");
            return View(vm);
        }

        if (!vm.HoverImage?.CheckType("image") ?? false)
        {
            SendItemsWithViewBag();
            ModelState.AddModelError("HoverImage", "Please add only image!");
            return View(vm);
        }

        if (vm.HoverImage?.CheckSize(2) ?? false)
        {
            SendItemsWithViewBag();
            ModelState.AddModelError("HoverImage", "Maximum 2 mb!");
            return View(vm);
        }

        foreach (var image in vm.Images)
        {
            if (!image.CheckType("image"))
            {
                SendItemsWithViewBag();
                ModelState.AddModelError("Images", "Please add only image!");
                return View(vm);
            }

            if (!image.CheckSize(2))
            {
                SendItemsWithViewBag();
                ModelState.AddModelError("Images", "Maximum 2 mb!");
                return View(vm);
            }
        }

        var existProduct = _context.Products.Include(x=>x.ProductTags).Include(x=>x.ProductImages).FirstOrDefault(x=>x.Id==vm.Id);

        if (existProduct is null)
        {
            return NotFound();
        }

        var isExistCategory = _context.Categories.Any(c => c.Id == vm.CategoryId);

        if (!isExistCategory)
        {
            SendItemsWithViewBag();
            ModelState.AddModelError("CategoryId", "Category not found!");
            return View(vm);
        }

        foreach (var tagId in vm.TagIds)
        {
            var isExistTag = _context.Tags.Any(x => x.Id == tagId);
            if (!isExistTag)
            {
                SendItemsWithViewBag();
                ModelState.AddModelError("", "This tag doesnot exist!");
                return View(vm);
            }
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

        existProduct.ProductTags = [];

        foreach (var tagId in vm.TagIds)
        {
            ProductTag productTag = new ProductTag()
            {
                TagId = tagId,
                ProductId=existProduct.Id
            };
            existProduct.ProductTags.Add(productTag);
        }

        foreach (var productImage in existProduct.ProductImages.ToList())
        {
            var isExistImage = vm.ImageIds.Any(x => x == productImage.Id);
            if (isExistImage == false)
            {
                existProduct.ProductImages.Remove(productImage);
                if (System.IO.File.Exists(Path.Combine(folderPath, productImage.ImageUrl)))
                {
                    System.IO.File.Delete(Path.Combine(folderPath, productImage.ImageUrl));
                }
            }
        }

        foreach (var image in vm.Images)
        {
            string imageName = image.SaveFile(folderPath);

            ProductImage productImage = new ProductImage()
            {
                ImageUrl = imageName,
                ProductId = existProduct.Id,
            };
            existProduct.ProductImages.Add(productImage);
        }

        _context.Update(existProduct);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));

    }

    public IActionResult Detail(int id)
    {
        var product = _context.Products.Select(x => new ProductGetVM()
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            Price = x.Price,
            Rating = x.Rating,
            CategoryName = x.Category.Name,
            BrandName = x.Brand.Name,
            MainImageUrl = x.MainImageUrl,
            HoverImageUrl = x.HoverImageUrl,
            SKU = x.SKU,
            ImageUrls=x.ProductImages.Select(x => x.ImageUrl).ToList(),
            TagNames = x.ProductTags.Select(x => x.Tag.Name).ToList()
        }).FirstOrDefault(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    private void SendItemsWithViewBag()
    {
        var categories = _context.Categories.ToList();
        ViewBag.Categories = categories;

        var tags = _context.Tags.ToList();
        ViewBag.Tags = tags;

        var brands= _context.Brands.ToList();
        ViewBag.Brands = brands;
    }
}
