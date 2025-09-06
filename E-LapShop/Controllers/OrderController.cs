using AutoMapper;
using BLL.Models.Order;
using BLL.Services.OrderServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_LapShop.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;
        private readonly IMapper _mapper;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger, IMapper mapper)
        {
            _orderService = orderService;
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
    }
}
