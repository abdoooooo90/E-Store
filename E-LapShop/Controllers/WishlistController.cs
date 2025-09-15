using BLL.Models.WishlistDtos;
using BLL.Services.WishlistServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DAL.Models;

namespace E_LapShop.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly IWishlistService _wishlistService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<WishlistController> _logger;

        public WishlistController(IWishlistService wishlistService, UserManager<ApplicationUser> userManager, ILogger<WishlistController> logger)
        {
            _wishlistService = wishlistService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var wishlistItems = await _wishlistService.GetUserWishlistAsync(userId);
            return View(wishlistItems);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] WishlistAddRequest request)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                    return Json(new { success = false, message = "Please log in to add items to wishlist." });

                // Check if already in wishlist
                var isAlreadyInWishlist = await _wishlistService.IsInWishlistAsync(userId, request.ProductId);
                if (isAlreadyInWishlist)
                    return Json(new { success = false, message = "Product is already in your wishlist." });

                var dto = new WishlistCreateDto
                {
                    UserId = userId,
                    ProductId = request.ProductId
                };

                var result = await _wishlistService.AddToWishlistAsync(dto);
                
                if (result)
                    return Json(new { success = true, message = "Product added to wishlist successfully!" });
                else
                    return Json(new { success = false, message = "Failed to add product to wishlist." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product to wishlist");
                return Json(new { success = false, message = "An error occurred while adding to wishlist." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int productId)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                var result = await _wishlistService.RemoveFromWishlistAsync(userId, productId);
                
                if (result)
                    TempData["Success"] = "Product removed from wishlist.";
                else
                    TempData["Error"] = "Failed to remove product from wishlist.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing product from wishlist");
                TempData["Error"] = "An error occurred while removing from wishlist.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                    return Json(new { success = false, message = "Please log in to add items to wishlist." });

                var isAlreadyInWishlist = await _wishlistService.IsInWishlistAsync(userId, productId);
                if (isAlreadyInWishlist)
                    return Json(new { success = false, message = "Product is already in your wishlist." });

                var dto = new WishlistCreateDto
                {
                    UserId = userId,
                    ProductId = productId
                };

                var result = await _wishlistService.AddToWishlistAsync(dto);
                
                if (result)
                    return Json(new { success = true, message = "Product added to wishlist successfully!" });
                else
                    return Json(new { success = false, message = "Failed to add product to wishlist." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product to wishlist");
                return Json(new { success = false, message = "An error occurred while adding to wishlist." });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                    return Json(new { success = false, message = "User not authenticated" });

                var result = await _wishlistService.RemoveFromWishlistAsync(userId, productId);
                
                return Json(new { success = result, message = result ? "Product removed from wishlist." : "Failed to remove product from wishlist." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing product from wishlist");
                return Json(new { success = false, message = "An error occurred while removing from wishlist." });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> IsInWishlist(int productId)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                    return Json(new { isInWishlist = false });

                var isInWishlist = await _wishlistService.IsInWishlistAsync(userId, productId);
                return Json(new { isInWishlist = isInWishlist });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking wishlist status");
                return Json(new { isInWishlist = false });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetWishlistCount()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                    return Json(new { count = 0 });

                var wishlistItems = await _wishlistService.GetUserWishlistAsync(userId);
                return Json(new { count = wishlistItems.Count() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting wishlist count");
                return Json(new { count = 0 });
            }
        }

        [HttpPost]
        public async Task<IActionResult> MoveToCart(int id, int quantity = 1)
        {
            try
            {
                var result = await _wishlistService.MoveToCartAsync(id, quantity);
                
                if (result)
                    TempData["Success"] = "Product moved to cart successfully!";
                else
                    TempData["Error"] = "Failed to move product to cart.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error moving product to cart");
                TempData["Error"] = "An error occurred while moving to cart.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCount()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                {
                    return Json(new { count = 0 });
                }

                var count = await _wishlistService.GetWishlistCountAsync(userId);
                return Json(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting wishlist count");
                return Json(new { count = 0 });
            }
        }
    }

    public class WishlistAddRequest
    {
        public int ProductId { get; set; }
    }
}
