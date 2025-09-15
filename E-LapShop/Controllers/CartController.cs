using BLL.Services.CartItemServices;
using BLL.Services.OrderServices;
using BLL.Models.CartItemDtos;
using BLL.Models.OrderDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DAL.Models;
using Microsoft.AspNetCore.Http;

namespace E_LapShop.Controllers
{
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
            {
                // Show empty cart page for non-authenticated users
                return View(new List<CartItemDto>());
            }

            var cartItems = await _cartItemService.GetUserCartAsync(userId);
            ViewBag.CartTotal = cartItems?.Sum(x => x.Total) ?? 0;
            return View(cartItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(CartAddRequest request)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                {
                    // Check if it's an AJAX request
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return Json(new { success = false, message = "يرجى تسجيل الدخول لإضافة المنتجات إلى العربة." });
                    
                    return RedirectToAction("Auth", "Account");
                }

                var dto = new CartItemCreateDto
                {
                    UserId = userId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                };

                _logger.LogInformation($"Attempting to add product {request.ProductId} (quantity: {request.Quantity}) to cart for user {userId}");
                
                var result = await _cartItemService.AddToCartAsync(dto);
                
                if (result)
                {
                    _logger.LogInformation($"Product {request.ProductId} successfully added to cart for user {userId}");
                    
                    // Check if it's an AJAX request
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return Json(new { success = true, message = "تم إضافة المنتج للسلة بنجاح" });
                    
                    // For regular form submission, redirect to shop
                    TempData["Success"] = "تم إضافة المنتج للسلة بنجاح";
                    return RedirectToAction("Shop", "Furni");
                }
                else
                {
                    _logger.LogWarning($"Failed to add product {request.ProductId} to cart for user {userId} - service returned false");
                    
                    // Check if it's an AJAX request
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return Json(new { success = false, message = "فشل في إضافة المنتج للسلة - قد يكون المخزون غير كافي" });
                    
                    TempData["Error"] = "فشل في إضافة المنتج للسلة - قد يكون المخزون غير كافي";
                    return RedirectToAction("Shop", "Furni");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding product {request.ProductId} to cart for user {_userManager.GetUserId(User)}: {ex.Message}");
                
                // Check if it's an AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = ex.Message });
                
                TempData["Error"] = ex.Message;
                return RedirectToAction("Shop", "Furni");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            try
            {
                // Get the cart item to check product stock
                var cartItem = await _cartItemService.GetByIdAsync(id);
                if (cartItem == null)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return Json(new { success = false, message = "Cart item not found" });
                    
                    TempData["Error"] = "Cart item not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Check if requested quantity exceeds available stock
                if (quantity > cartItem.ProductStock)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return Json(new { 
                            success = false, 
                            message = $"Cannot add more items. Only {cartItem.ProductStock} items available in stock.",
                            maxQuantity = cartItem.ProductStock
                        });
                    
                    TempData["Error"] = $"Cannot add more items. Only {cartItem.ProductStock} items available in stock.";
                    return RedirectToAction(nameof(Index));
                }

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
            
            // Get discount information from session
            var couponCode = HttpContext.Session.GetString("CouponCode");
            var discountPercentageStr = HttpContext.Session.GetString("DiscountPercentage");
            var discountAmountStr = HttpContext.Session.GetString("DiscountAmount");
            
            decimal discountAmount = 0;
            int discountPercentage = 0;
            decimal finalTotal = cartTotal;
            
            if (!string.IsNullOrEmpty(couponCode) && !string.IsNullOrEmpty(discountAmountStr))
            {
                decimal.TryParse(discountAmountStr, out discountAmount);
                int.TryParse(discountPercentageStr, out discountPercentage);
                finalTotal = cartTotal - discountAmount;
            }
            
            ViewBag.CartTotal = cartTotal;
            ViewBag.CartItems = cartItems;
            ViewBag.CouponCode = couponCode;
            ViewBag.DiscountPercentage = discountPercentage;
            ViewBag.DiscountAmount = discountAmount;
            ViewBag.FinalTotal = finalTotal;
            
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

                // Calculate total with discount
                var cartTotal = cartItems.Sum(item => item.Total);
                var discountAmountStr = HttpContext.Session.GetString("DiscountAmount");
                decimal discountAmount = 0;
                if (!string.IsNullOrEmpty(discountAmountStr))
                {
                    decimal.TryParse(discountAmountStr, out discountAmount);
                }
                var totalAmount = cartTotal - discountAmount;

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

                    // Clear cart and discount session after successful order
                    foreach (var item in cartItems)
                    {
                        await _cartItemService.DeleteAsync(item.Id);
                    }
                    
                    // Clear discount session data
                    HttpContext.Session.Remove("CouponCode");
                    HttpContext.Session.Remove("DiscountPercentage");
                    HttpContext.Session.Remove("DiscountAmount");

                    return Json(new { success = true, message = "تم إنشاء الطلب بنجاح", orderId = createdOrder.Id, redirectUrl = Url.Action("ThankYou", "Cart") });
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

        public IActionResult ThankYou()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(string couponCode)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                {
                    return Json(new { success = false, message = "Please log in to apply a coupon." });
                }

                if (string.IsNullOrWhiteSpace(couponCode))
                {
                    return Json(new { success = false, message = "Please enter a coupon code." });
                }

                // Accept ITI10, ITI20, ITI30, ITI40, ITI50, ITI60 only
                var regex = new System.Text.RegularExpressions.Regex(@"^ITI(10|20|30|40|50|60)$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                var match = regex.Match(couponCode);

                if (!match.Success)
                {
                    // Clear any existing coupon session data for invalid coupons
                    HttpContext.Session.Remove("CouponCode");
                    HttpContext.Session.Remove("DiscountPercentage");
                    HttpContext.Session.Remove("DiscountAmount");
                    
                    return Json(new { success = false, message = "Invalid coupon code. Allowed: ITI10, ITI20, ITI30, ITI40, ITI50, ITI60." });
                }

                var discountPercentage = int.Parse(match.Groups[1].Value);
                var discountRate = discountPercentage / 100.0m;

                var cartTotal = await _cartItemService.GetCartTotalAsync(userId);
                var discountAmount = cartTotal * discountRate;
                var newTotal = cartTotal - discountAmount;

                // Store discount information in session
                HttpContext.Session.SetString("CouponCode", couponCode.ToUpper());
                HttpContext.Session.SetString("DiscountPercentage", discountPercentage.ToString());
                HttpContext.Session.SetString("DiscountAmount", discountAmount.ToString("F2"));

                return Json(new
                {
                    success = true,
                    message = $"Coupon '{couponCode.ToUpper()}' applied successfully! ({discountPercentage}% OFF)",
                    discountAmount = discountAmount,
                    newTotal = newTotal
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying coupon {couponCode}", couponCode);
                return Json(new { success = false, message = "An error occurred while applying the coupon." });
            }
        }

    }

    public class CartAddRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
