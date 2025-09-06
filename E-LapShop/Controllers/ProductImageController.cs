using BLL.Models.ProductImageDtos;
using BLL.Services.ProductImageServices;
using Microsoft.AspNetCore.Mvc;

namespace E_LapShop.Controllers
{
    public class ProductImageController : Controller
    {
        private readonly IProductImageService _productImageService;

        public ProductImageController(IProductImageService productImageService)
        {
            _productImageService = productImageService;
        }


        public async Task<IActionResult> Index()
        {
            var images = await _productImageService.GetAllAsync();
            return View(images);
        }


        public async Task<IActionResult> Details(int id)
        {
            var image = await _productImageService.GetByIdAsync(id);
            if (image == null)
                return NotFound();

            return View(image);
        }


        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductImageCreateDto dto)
        {
            if (ModelState.IsValid)
            {
                await _productImageService.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }


        public async Task<IActionResult> Edit(int id)
        {
            var image = await _productImageService.GetByIdAsync(id);
            if (image == null)
                return NotFound();

            var updateDto = new ProductImageUpdateDto
            {
                ImageUrl = image.ImageUrl,
                ProductId = image.ProductId
            };

            return View(updateDto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductImageUpdateDto dto)
        {
            if (ModelState.IsValid)
            {
                var updated = await _productImageService.UpdateAsync(id, dto);
                if (!updated) return NotFound();

                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var image = await _productImageService.GetByIdAsync(id);
            if (image == null)
                return NotFound();

            return View(image);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productImageService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
