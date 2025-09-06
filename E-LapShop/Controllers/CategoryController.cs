using BLL.Models.CategoryDtos;
using BLL.Services.CategorieServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_LapShop.Controllers
{
        [Authorize]
        public class CategoryController : Controller
        {
            private readonly ICategoryService _categoryService;

            public CategoryController(ICategoryService categoryService)
            {
                _categoryService = categoryService;
            }

            
            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> Index()
            {
                var categories = await _categoryService.GetAllAsync();
                return View(categories); 
            }

           
            public async Task<IActionResult> Details(int id)
            {
                var category = await _categoryService.GetByIdAsync(id);
                if (category == null)
                    return NotFound();

                return View(category);
            }

           
            [Authorize(Roles = "Admin")]
            public IActionResult Create()
            {
                return View();
            }

            
            [HttpPost]
            [ValidateAntiForgeryToken]
            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> Create(CategoryCreateDto dto)
            {
                if (!ModelState.IsValid)
                    return View(dto);

                await _categoryService.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }

       
            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> Edit(int id)
            {
                var category = await _categoryService.GetByIdAsync(id);
                if (category == null)
                    return NotFound();

                
                var updateDto = new CategoryUpdateDto
                {
                    Name = category.Name,
                   // Description = category.Description
                };

                return View(updateDto);
            }

            
            [HttpPost]
            [ValidateAntiForgeryToken]
            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> Edit(int id, CategoryUpdateDto dto)
            {
                if (!ModelState.IsValid)
                    return View(dto);

                var result = await _categoryService.UpdateAsync(id, dto);
                if (!result)
                    return NotFound();

                return RedirectToAction(nameof(Index));
            }

            
            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> Delete(int id)
            {
                var category = await _categoryService.GetByIdAsync(id);
                if (category == null)
                    return NotFound();

                return View(category);
            }

            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                await _categoryService.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
        }
}
