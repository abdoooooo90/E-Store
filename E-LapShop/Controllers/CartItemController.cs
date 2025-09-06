using BLL.Models.CartItemDtos;
using BLL.Services.CartItemServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_LapShop.Controllers
{
        [Authorize]
        public class CartItemController : Controller
        {
            private readonly ICartItemService _cartItemService;

            public CartItemController(ICartItemService cartItemService)
            {
                _cartItemService = cartItemService;
            }

            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> Index()
            {
                var items = await _cartItemService.GetAllAsync();
                return View(items); 
            }

            
            public async Task<IActionResult> Details(int id)
            {
                var item = await _cartItemService.GetByIdAsync(id);
                if (item == null)
                    return NotFound();

                return View(item);
            }

            
            public IActionResult Create()
            {
                return View();
            }

            
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(CartItemDto dto)
            {
                if (!ModelState.IsValid)
                    return View(dto);

                await _cartItemService.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }

            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> Delete(int id)
            {
                var item = await _cartItemService.GetByIdAsync(id);
                if (item == null)
                    return NotFound();

                return View(item);
            }

         
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                await _cartItemService.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
        }
    }


