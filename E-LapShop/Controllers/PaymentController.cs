using BLL.Models.PaymentDtos;
using BLL.Services.PaymentServices;
using Microsoft.AspNetCore.Mvc;

namespace E_LapShop.Controllers
{
        public class PaymentController : Controller
        {
            private readonly IPaymentService _paymentService;

            public PaymentController(IPaymentService paymentService)
            {
                _paymentService = paymentService;
            }

            public async Task<IActionResult> Index()
            {
                var payments = await _paymentService.GetAllAsync();
                return View(payments);
            }

            public async Task<IActionResult> Details(int id)
            {
                var payment = await _paymentService.GetByIdAsync(id);
                if (payment == null)
                    return NotFound();

                return View(payment);
            }

           
            public IActionResult Create()
            {
                return View();
            }

            
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(PaymentCreateDto dto)
            {
                if (ModelState.IsValid)
                {
                    await _paymentService.CreateAsync(dto);
                    return RedirectToAction(nameof(Index));
                }
                return View(dto);
            }

           
            public async Task<IActionResult> Edit(int id)
            {
                var payment = await _paymentService.GetByIdAsync(id);
                if (payment == null)
                    return NotFound();

             
                return View(new PaymentUpdateStatusDto { PaymentId = id, Status = payment.Status });
            }

           
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(PaymentUpdateStatusDto dto)
            {
                if (ModelState.IsValid)
                {
                    var updated = await _paymentService.UpdateStatusAsync(dto);
                    if (!updated) return NotFound();

                    return RedirectToAction(nameof(Index));
                }
                return View(dto);
            }

            public async Task<IActionResult> Delete(int id)
            {
                var payment = await _paymentService.GetByIdAsync(id);
                if (payment == null)
                    return NotFound();

                return View(payment);
            }

         
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                await _paymentService.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
        }
    }

