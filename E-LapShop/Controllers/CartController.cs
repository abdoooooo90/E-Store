using BLL.Models.CartItemDtos;
using BLL.Services.CartItemServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DAL.Models;

namespace E_LapShop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartItemService _cartItemService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartItemService cartItemService, UserManager<ApplicationUser> userManager, ILogger<CartController> logger)
        {
            _cartItemService = cartItemService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var cartItems = await _cartItemService.GetUserCartAsync(userId);
            var cartTotal = await _cartItemService.GetCartTotalAsync(userId);
            
            ViewBag.CartTotal = cartTotal;
            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] CartAddRequest request)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                    return Json(new { success = false, message = "Please log in to add items to cart." });

                var dto = new CartItemCreateDto
                {
                    UserId = userId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                };

                var result = await _cartItemService.AddToCartAsync(dto);
                
                if (result)
                    return Json(new { success = true, message = "Product added to cart successfully!" });
                else
                    return Json(new { success = false, message = "Failed to add product to cart." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product to cart");
                return Json(new { success = false, message = "An error occurred while adding to cart." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            try
            {
                var result = await _cartItemService.UpdateQuantityAsync(id, quantity);
                
                if (result)
                    TempData["Success"] = "Cart updated successfully.";
                else
                    TempData["Error"] = "Failed to update cart.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart quantity");
                TempData["Error"] = "An error occurred while updating cart.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            try
            {
                var result = await _cartItemService.DeleteAsync(id);
                
                if (result)
                    TempData["Success"] = "Product removed from cart.";
                else
                    TempData["Error"] = "Failed to remove product from cart.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing product from cart");
                TempData["Error"] = "An error occurred while removing from cart.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                    return RedirectToAction("Login", "Account");

                var result = await _cartItemService.ClearUserCartAsync(userId);
                
                if (result)
                    TempData["Success"] = "Cart cleared successfully.";
                else
                    TempData["Error"] = "Failed to clear cart.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart");
                TempData["Error"] = "An error occurred while clearing cart.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                    return Json(new { count = 0 });

                var count = await _cartItemService.GetCartCountAsync(userId);
                return Json(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart count");
                return Json(new { count = 0 });
            }
        }

        public async Task<IActionResult> Checkout()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var cartItems = await _cartItemService.GetUserCartAsync(userId);
            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction(nameof(Index));
            }

            var cartTotal = await _cartItemService.GetCartTotalAsync(userId);
            ViewBag.CartTotal = cartTotal;
            ViewBag.CartItems = cartItems;
            
            return View();
        }
    }

    public class CartAddRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
