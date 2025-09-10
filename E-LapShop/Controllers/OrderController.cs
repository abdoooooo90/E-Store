using AutoMapper;
using BLL.Models.OrderDtos;
using BLL.Services.OrderServices;
using BLL.Services.CartItemServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DAL.Models;

namespace E_LapShop.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartItemService _cartItemService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<OrderController> _logger;
        private readonly IMapper _mapper;

        public OrderController(IOrderService orderService, ICartItemService cartItemService, UserManager<ApplicationUser> userManager, ILogger<OrderController> logger, IMapper mapper)
        {
            _orderService = orderService;
            _cartItemService = cartItemService;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
        }

         
        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllAsync();

            if (User.IsInRole("Admin"))
            {
                return View("IndexAdmin", orders);  
            }
            else
            {
                var userId = User.Identity!.Name;
                var userOrders = orders.Where(o => o.UserId == userId);
                return View("IndexUser", userOrders);  
            }
        }

         
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            if (!User.IsInRole("Admin") && order.UserId != User.Identity!.Name)
                return Forbid();

            return View(order);
        }

       
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderCreateDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            try
            {
                dto.UserId = User.Identity!.Name!;  
                var createdOrder = await _orderService.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                ModelState.AddModelError("", "Error creating order");
                return View(dto);
            }
        }

       
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            if (!User.IsInRole("Admin") && order.UserId != User.Identity!.Name)
                return Forbid();

            var dto = _mapper.Map<OrderUpdateDto>(order);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderUpdateDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            if (!User.IsInRole("Admin") && order.UserId != User.Identity!.Name)
                return Forbid();

            await _orderService.UpdateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

         
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            if (!User.IsInRole("Admin") && order.UserId != User.Identity!.Name)
                return Forbid();

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            if (!User.IsInRole("Admin") && order.UserId != User.Identity!.Name)
                return Forbid();

            await _orderService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(OrderCreateDto dto)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                    return RedirectToAction("Login", "Account");

                // Get cart items
                var cartItems = await _cartItemService.GetUserCartAsync(userId);
                if (!cartItems.Any())
                {
                    TempData["Error"] = "Your cart is empty.";
                    return RedirectToAction("Index", "Cart");
                }

                // Set user ID and calculate total
                dto.UserId = userId;
                dto.TotalAmount = await _cartItemService.GetCartTotalAsync(userId);
                dto.TotalAmount *= 1.1m; // Add 10% tax

                // Convert cart items to order items
                dto.Items = cartItems.Select(c => new OrderItemCreateDto
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    Price = c.ProductPrice
                }).ToList();

                // Create the order
                var createdOrder = await _orderService.CreateAsync(dto);
                
                if (createdOrder != null)
                {
                    // Clear the cart after successful order
                    await _cartItemService.ClearUserCartAsync(userId);
                    
                    TempData["Success"] = "Your order has been placed successfully!";
                    return RedirectToAction("OrderConfirmation", new { orderId = createdOrder.Id });
                }
                else
                {
                    TempData["Error"] = "Failed to place order. Please try again.";
                    return RedirectToAction("Checkout", "Cart");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error placing order");
                TempData["Error"] = "An error occurred while placing your order. Please try again.";
                return RedirectToAction("Checkout", "Cart");
            }
        }

        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var order = await _orderService.GetByIdAsync(orderId);
            if (order == null || order.UserId != userId)
                return NotFound();

            return View(order);
        }
    }
}
