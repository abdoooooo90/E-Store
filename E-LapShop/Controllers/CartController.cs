using BLL.Services.CartItemServices;
using BLL.Services.OrderServices;
using BLL.Models.CartItemDtos;
using BLL.Models.OrderDtos;
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
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartItemService cartItemService, IOrderService orderService, UserManager<ApplicationUser> userManager, ILogger<CartController> logger)
        {
            _cartItemService = cartItemService;
            _orderService = orderService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return RedirectToAction("Auth", "Account");

            var cartItems = await _cartItemService.GetUserCartAsync(userId);
            ViewBag.CartTotal = cartItems?.Sum(x => x.Total) ?? 0;
            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] CartAddRequest request)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                    return Json(new { success = false, message = "يرجى تسجيل الدخول لإضافة المنتجات إلى العربة." });

                var dto = new CartItemCreateDto
                {
                    UserId = userId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                };

                var result = await _cartItemService.AddToCartAsync(dto);
                
                if (result)
                {
                    // Log successful addition for debugging
                    _logger.LogInformation($"Product {request.ProductId} added to cart for user {userId}");
                    return Json(new { success = true, message = "تم إضافة المنتج للسلة بنجاح" });
                }
                else
                {
                    _logger.LogWarning($"Failed to add product {request.ProductId} to cart for user {userId}");
                    return Json(new { success = false, message = "فشل في إضافة المنتج للسلة - قد يكون المخزون غير كافي" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product to cart");
                return Json(new { success = false, message = "حدث خطأ أثناء إضافة المنتج إلى العربة." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            try
            {
                var result = await _cartItemService.UpdateQuantityAsync(id, quantity);
                
                if (result)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return Json(new { success = true, message = "Cart updated successfully" });
                    
                    TempData["Success"] = "Cart updated successfully.";
                }
                else
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return Json(new { success = false, message = "Failed to update cart" });
                    
                    TempData["Error"] = "Failed to update cart.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart quantity");
                
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "An error occurred while updating cart" });
                
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
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return Json(new { success = true, message = "Product removed from cart" });
                    
                    TempData["Success"] = "Product removed from cart.";
                }
                else
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return Json(new { success = false, message = "Failed to remove product from cart" });
                    
                    TempData["Error"] = "Failed to remove product from cart.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing product from cart");
                
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "An error occurred while removing from cart" });
                
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

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string shippingAddress, string paymentMethod)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                {
                    return Json(new { success = false, message = "يجب تسجيل الدخول أولاً" });
                }

                // Get cart items
                var cartItems = await _cartItemService.GetUserCartAsync(userId);
                if (!cartItems.Any())
                {
                    return Json(new { success = false, message = "السلة فارغة" });
                }

                // Calculate total
                var totalAmount = cartItems.Sum(item => item.Total);

                // Create shipping address DTO
                var shippingAddressDto = new ShippingAddressDto
                {
                    FirstName = "Customer",
                    LastName = "Name",
                    Street = shippingAddress,
                    City = "City",
                    State = "State",
                    ZipCode = "00000",
                    Country = "Country"
                };

                // Create order
                var orderDto = new OrderCreateDto
                {
                    UserId = userId,
                    TotalAmount = totalAmount,
                    ShippingAddress = shippingAddressDto,
                    PaymentMethod = paymentMethod,
                    Items = cartItems.Select(item => new OrderItemCreateDto
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.ProductPrice
                    }).ToList()
                };

                var createdOrder = await _orderService.CreateAsync(orderDto);

                if (createdOrder != null)
                {
                    // Log successful order creation
                    _logger.LogInformation($"Order {createdOrder.Id} created successfully for user {userId}");

                    // Clear cart after successful order
                    foreach (var item in cartItems)
                    {
                        await _cartItemService.DeleteAsync(item.Id);
                    }

                    return Json(new { success = true, message = "تم إنشاء الطلب بنجاح", orderId = createdOrder.Id });
                }
                else
                {
                    _logger.LogError("Failed to create order - service returned null");
                    return Json(new { success = false, message = "فشل في إنشاء الطلب" });
                }
            }
            catch (Exception ex)
            {
                var userId = _userManager.GetUserId(User);
                _logger.LogError(ex, "Error creating order for user {UserId}", userId);
                return Json(new { success = false, message = "حدث خطأ أثناء إنشاء الطلب: " + ex.Message });
            }
        }

        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var order = await _orderService.GetByIdAsync(orderId);
            if (order == null)
                return NotFound();

            return View(order);
        }
    }

    public class CartAddRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
