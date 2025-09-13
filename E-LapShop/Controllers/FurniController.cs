using BLL.Services.ProductServices;
using BLL.Services.CategorieServices;
using E_LapShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace E_LapShop.Controllers
{
    public class FurniController : Controller
    {
        private readonly ILogger<FurniController> _logger;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public FurniController(ILogger<FurniController> logger, IProductService productService, ICategoryService categoryService)
        {
            _logger = logger;
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            // Get the latest 3 products ordered by creation date
            var latestProducts = await _productService.GetLatestProductsAsync(3);
            return View(latestProducts);
        }

        public async Task<IActionResult> Shop(int? categoryId, string searchTerm)
        {
            var products = await _productService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();
            
            // Apply filters
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                             (p.Description != null && p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
            }
            
            ViewBag.Categories = categories;
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.SearchTerm = searchTerm;
            
            return View(products);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Services()
        {
            return View();
        }

        public IActionResult Blog()
        {
            return View();
        } 

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Cart()
        {
            return View();
        }

        public IActionResult Checkout()
        {
            return View();
        }

        public IActionResult ThankYou()
        {
            return View();
        }

        public async Task<IActionResult> ProductDetails(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            
            // Get related products (same category, excluding current product)
            var allProducts = await _productService.GetAllAsync();
            var relatedProducts = allProducts
                .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)
                .Take(3)
                .ToList();
            
            ViewBag.RelatedProducts = relatedProducts;
            return View(product);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
