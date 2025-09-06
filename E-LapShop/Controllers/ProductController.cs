using AutoMapper;
using BLL.Models.ProductDtos;
using BLL.Services.ProductService.ProductService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_LapShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;

        public ProductController(IProductService productService, ILogger<ProductController> logger,
                                 IWebHostEnvironment environment, IMapper mapper)
        {
            _productService = productService;
            _logger = logger;
            _environment = environment;
            _mapper = mapper;
        }

        #region Index
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllAsync();
            return View(products);
        }
        #endregion

        #region Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null) return BadRequest();

            var product = await _productService.GetByIdAsync(id.Value);
            if (product is null) return NotFound();

            return View(product);
        }
        #endregion

        #region Create
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create() => View();
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            try
            {
                var createdProduct = await _productService.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                var message = _environment.IsDevelopment() ? ex.Message : "An error occurred while creating the product.";
                ModelState.AddModelError(string.Empty, message);
                return View(dto);
            }
        }
        #endregion

        #region Edit
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return BadRequest();

            var product = await _productService.GetByIdAsync(id.Value);
            if (product is null) return NotFound();

            var dto = _mapper.Map<ProductUpdateDto>(product);
            return View(dto);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductUpdateDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            try
            {
                var updated = await _productService.UpdateAsync(id, dto);
                if (updated) return RedirectToAction(nameof(Index));

                ModelState.AddModelError(string.Empty, "An error occurred while updating the product.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                var message = _environment.IsDevelopment() ? ex.Message : "An error occurred while updating the product.";
                ModelState.AddModelError(string.Empty, message);
            }

            return View(dto);
        }
        #endregion

        #region Delete
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();

            var product = await _productService.GetByIdAsync(id.Value);
            if (product is null) return NotFound();

            return View(product);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var deleted = await _productService.DeleteAsync(id);
                if (deleted) return RedirectToAction(nameof(Index));

                TempData["Error"] = "An error occurred while deleting the product.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                TempData["Error"] = _environment.IsDevelopment() ? ex.Message : "An error occurred while deleting the product.";
            }

            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
