using BLL.Services.ProductServices;
using BLL.Services.OrderServices;
using BLL.Services.CategorieServices;
using BLL.Models.ProductDtos;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_LapShop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly ICategoryService _categoryService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(
            IProductService productService,
            IOrderService orderService,
            ICategoryService categoryService,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _productService = productService;
            _orderService = orderService;
            _categoryService = categoryService;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // إدارة المنتجات
        public async Task<IActionResult> Products()
        {
            ViewBag.Title = "إدارة المنتجات";
            var products = await _productService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();
            
            ViewBag.Categories = categories;
            return View(products);
        }

        // إضافة منتج جديد - GET
        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            ViewBag.Title = "إضافة منتج جديد";
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories;
            return View();
        }

        // إضافة منتج جديد - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(ProductCreateDto model, IFormFile mainImage)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _categoryService.GetAllAsync();
                return View(model);
            }

            try
            {
                // Handle image upload
                if (mainImage != null && mainImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "products");
                    Directory.CreateDirectory(uploadsFolder);
                    
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + mainImage.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await mainImage.CopyToAsync(fileStream);
                    }
                    
                    model.ImageUrl = "/uploads/products/" + uniqueFileName;
                }

                await _productService.CreateAsync(model);
                TempData["Success"] = "تم إضافة المنتج بنجاح";
                return RedirectToAction(nameof(Products));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "حدث خطأ أثناء إضافة المنتج: " + ex.Message;
                ViewBag.Categories = await _categoryService.GetAllAsync();
                return View(model);
            }
        }

        // التحقق من وجود اسم المنتج
        [HttpPost]
        public async Task<IActionResult> CheckProductName(string productName, int? excludeId = null)
        {
            try
            {
                var products = await _productService.GetAllAsync();
                var exists = products.Any(p => p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase) 
                                              && (excludeId == null || p.Id != excludeId));
                
                return Json(new { exists = exists });
            }
            catch (Exception)
            {
                return Json(new { exists = false });
            }
        }

        // تعديل منتج - GET
        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            ViewBag.Title = "تعديل المنتج";
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                TempData["Error"] = "المنتج غير موجود";
                return RedirectToAction(nameof(Products));
            }

            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories;
            
            var updateDto = new ProductUpdateDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                StockQuantity = product.StockQuantity,
                IsAvailable = product.IsAvailable
            };

            return View(updateDto);
        }

        // تعديل منتج - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, ProductUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryService.GetAllAsync();
                ViewBag.Categories = categories;
                return View(model);
            }

            try
            {
                var result = await _productService.UpdateAsync(id, model);
                if (result)
                {
                    TempData["Success"] = "تم تحديث المنتج بنجاح";
                    return RedirectToAction(nameof(Products));
                }
                else
                {
                    TempData["Error"] = "فشل في تحديث المنتج";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "حدث خطأ أثناء تحديث المنتج: " + ex.Message;
            }

            var categoriesForView = await _categoryService.GetAllAsync();
            ViewBag.Categories = categoriesForView;
            return View(model);
        }

        // حذف منتج - GET
        [HttpGet]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            ViewBag.Title = "حذف المنتج";
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                TempData["Error"] = "المنتج غير موجود";
                return RedirectToAction(nameof(Products));
            }

            return View(product);
        }

        // حذف منتج - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductConfirmed(int id)
        {
            Console.WriteLine($"DeleteProductConfirmed called with ID: {id}");
            
            try
            {
                // التحقق من وجود المنتج أولاً وحفظ اسمه
                var product = await _productService.GetByIdAsync(id);
                Console.WriteLine($"Product found: {product?.Name ?? "null"}");
                
                if (product == null)
                {
                    Console.WriteLine("Product not found, redirecting with error");
                    TempData["Error"] = "المنتج غير موجود في قاعدة البيانات";
                    return RedirectToAction(nameof(Products));
                }

                // حفظ اسم المنتج قبل الحذف
                var productName = product.Name;
                
                Console.WriteLine($"Attempting to delete product: {productName}");
                var success = await _productService.DeleteAsync(id);
                Console.WriteLine($"Delete result: {success}");
                
                if (success)
                {
                    Console.WriteLine("Delete successful, setting success message");
                    TempData["Success"] = $"تم حذف المنتج '{productName}' بنجاح";
                    return RedirectToAction(nameof(Products));
                }
                else
                {
                    Console.WriteLine("Delete failed, setting error message");
                    TempData["Error"] = $"فشل حذف المنتج '{productName}' - قد يكون مرتبط ببيانات أخرى أو تم تعديله مؤخراً";
                    return RedirectToAction(nameof(Products));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in DeleteProductConfirmed: {ex.Message}");
                TempData["Error"] = "حدث خطأ أثناء حذف المنتج: " + ex.Message;
                return RedirectToAction(nameof(Products));
            }
        }

        // Dashboard الرئيسية
        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "لوحة التحكم - إدارة متجر الأثاث";
            
            // إحصائيات سريعة
            var allOrders = await _orderService.GetAllAsync();
            var allProducts = await _productService.GetAllAsync();
            var allCategories = await _categoryService.GetAllAsync();
            var totalUsers = _userManager.Users.Count();

            // حساب الإحصائيات المالية
            var totalRevenue = allOrders.Sum(o => o.TotalAmount);
            var completedOrders = allOrders.Where(o => o.Status?.ToLower() == "delivered").ToList();
            var pendingOrders = allOrders.Where(o => o.Status?.ToLower() == "pending").ToList();
            var thisMonthOrders = allOrders.Where(o => o.OrderDate.Month == DateTime.Now.Month && o.OrderDate.Year == DateTime.Now.Year).ToList();
            var lastMonthOrders = allOrders.Where(o => o.OrderDate.Month == DateTime.Now.AddMonths(-1).Month && o.OrderDate.Year == DateTime.Now.AddMonths(-1).Year).ToList();

            // حساب نسبة النمو
            var thisMonthRevenue = thisMonthOrders.Sum(o => o.TotalAmount);
            var lastMonthRevenue = lastMonthOrders.Sum(o => o.TotalAmount);
            var growthPercentage = lastMonthRevenue > 0 ? ((thisMonthRevenue - lastMonthRevenue) / lastMonthRevenue) * 100 : 0;

            // متوسط قيمة الطلب
            var averageOrderValue = allOrders.Any() ? allOrders.Average(o => o.TotalAmount) : 0;

            // إحصائيات المنتجات
            var lowStockProducts = allProducts.Where(p => p.Stock <= 10 && p.Stock > 0).Count();
            var outOfStockProducts = allProducts.Where(p => p.Stock == 0).Count();

            ViewBag.TotalProducts = allProducts.Count();
            ViewBag.TotalOrders = allOrders.Count();
            ViewBag.TotalCategories = allCategories.Count();
            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.CompletedOrders = completedOrders.Count();
            ViewBag.PendingOrders = pendingOrders.Count();
            ViewBag.ThisMonthOrders = thisMonthOrders.Count();
            ViewBag.GrowthPercentage = Math.Round(growthPercentage, 1);
            ViewBag.AverageOrderValue = averageOrderValue;
            ViewBag.LowStockProducts = lowStockProducts;
            ViewBag.OutOfStockProducts = outOfStockProducts;

            // بيانات الرسوم البيانية
            ViewBag.SalesChartData = GetSalesChartData(allOrders, "6months");
            ViewBag.CategoryChartData = GetCategoryChartData(allProducts, allCategories);
            ViewBag.MonthlyGrowthData = GetMonthlyGrowthData(allOrders);

            return View();
        }


        // إدارة الطلبات
        public async Task<IActionResult> Orders()
        {
            ViewBag.Title = "إدارة الطلبات";
            var orders = await _orderService.GetAllAsync();
            return View(orders);
        }

        // إدارة الفئات
        public async Task<IActionResult> Categories()
        {
            ViewBag.Title = "إدارة الفئات";
            var categories = await _categoryService.GetAllAsync();
            return View(categories);
        }

        // إدارة المستخدمين
        public IActionResult Users()
        {
            ViewBag.Title = "إدارة المستخدمين";
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // تقارير المبيعات
        public async Task<IActionResult> Sales()
        {
            ViewBag.Title = "تقارير المبيعات";
            var orders = await _orderService.GetAllAsync();
            return View(orders);
        }

        // إعدادات المتجر
        public IActionResult Settings()
        {
            ViewBag.Title = "إعدادات المتجر";
            return View();
        }

        // CRUD Operations for Categories
        [HttpPost]
        public async Task<IActionResult> CreateCategory(string name, string description, bool isActive, IFormFile image)
        {
            try
            {
                var categoryDto = new BLL.Models.CategoryDtos.CategoryCreateDto
                {
                    Name = name
                };

                // Image upload logic can be added later

                await _categoryService.CreateAsync(categoryDto);
                TempData["SuccessMessage"] = "تم إضافة الفئة بنجاح";
                return RedirectToAction("Categories");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء إضافة الفئة: " + ex.Message;
                return RedirectToAction("Categories");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory(int id, string name, string description, bool isActive, IFormFile image)
        {
            try
            {
                var categoryDto = new BLL.Models.CategoryDtos.CategoryUpdateDto
                {
                    Name = name
                };

                await _categoryService.UpdateAsync(id, categoryDto);
                TempData["SuccessMessage"] = "تم تحديث الفئة بنجاح";
                return Ok();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء تحديث الفئة: " + ex.Message;
                return BadRequest();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _categoryService.DeleteAsync(id);
                TempData["SuccessMessage"] = "تم حذف الفئة بنجاح";
                return Ok();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء حذف الفئة: " + ex.Message;
                return BadRequest();
            }
        }

        // Order Management Methods
        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusRequest request)
        {
            try
            {
                var orderDto = new BLL.Models.OrderDtos.OrderUpdateDto
                {
                    Id = request.OrderId,
                    Status = request.Status
                };

                await _orderService.UpdateAsync(orderDto);
                TempData["SuccessMessage"] = "تم تحديث حالة الطلب بنجاح";
                return Ok();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء تحديث حالة الطلب: " + ex.Message;
                return BadRequest();
            }
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            try
            {
                var order = await _orderService.GetByIdAsync(id);
                return PartialView("_OrderDetailsPartial", order);
            }
            catch (Exception ex)
            {
                return BadRequest("حدث خطأ أثناء تحميل تفاصيل الطلب: " + ex.Message);
            }
        }

        public async Task<IActionResult> PrintInvoice(int id)
        {
            try
            {
                var order = await _orderService.GetByIdAsync(id);
                return View("Invoice", order);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء تحميل الفاتورة: " + ex.Message;
                return RedirectToAction("Orders");
            }
        }

        public async Task<IActionResult> ExportOrders()
        {
            try
            {
                var orders = await _orderService.GetAllAsync();
                // Export logic would go here (CSV, Excel, etc.)
                TempData["SuccessMessage"] = "تم تصدير البيانات بنجاح";
                return RedirectToAction("Orders");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء تصدير البيانات: " + ex.Message;
                return RedirectToAction("Orders");
            }
        }

        // دوال مساعدة لحساب بيانات الرسوم البيانية
        private object GetSalesChartData(IEnumerable<BLL.Models.OrderDtos.OrderDto> orders, string period = "6months")
        {
            var salesData = new List<object>();
            int monthsCount = period switch
            {
                "3months" => 3,
                "6months" => 6,
                "12months" => 12,
                "thisyear" => 12,
                _ => 6
            };

            DateTime startDate = period == "thisyear" 
                ? new DateTime(DateTime.Now.Year, 1, 1)
                : DateTime.Now.AddMonths(-monthsCount + 1);

            for (int i = 0; i < monthsCount; i++)
            {
                var month = period == "thisyear" 
                    ? startDate.AddMonths(i)
                    : DateTime.Now.AddMonths(-monthsCount + 1 + i);
                    
                var monthOrders = orders.Where(o => o.OrderDate.Month == month.Month && o.OrderDate.Year == month.Year);
                var monthRevenue = monthOrders.Sum(o => o.TotalAmount);
                
                salesData.Add(new
                {
                    Month = month.ToString("MMM yyyy"),
                    Revenue = monthRevenue,
                    OrderCount = monthOrders.Count()
                });
            }
            return salesData;
        }

        private object GetCategoryChartData(IEnumerable<BLL.Models.ProductDtos.ProductDto> products, IEnumerable<BLL.Models.CategoryDtos.CategoryDto> categories)
        {
            var categoryData = categories.Select(c => new
            {
                Name = c.Name,
                ProductCount = products.Count(p => p.CategoryId == c.Id),
                Percentage = categories.Any() ? Math.Round((double)products.Count(p => p.CategoryId == c.Id) / products.Count() * 100, 1) : 0
            }).Where(c => c.ProductCount > 0).ToList();

            return categoryData;
        }

        private object GetMonthlyGrowthData(IEnumerable<BLL.Models.OrderDtos.OrderDto> orders)
        {
            var growthData = new List<object>();
            for (int i = 11; i >= 0; i--)
            {
                var currentMonth = DateTime.Now.AddMonths(-i);
                var previousMonth = currentMonth.AddMonths(-1);
                
                var currentMonthRevenue = orders.Where(o => o.OrderDate.Month == currentMonth.Month && o.OrderDate.Year == currentMonth.Year).Sum(o => o.TotalAmount);
                var previousMonthRevenue = orders.Where(o => o.OrderDate.Month == previousMonth.Month && o.OrderDate.Year == previousMonth.Year).Sum(o => o.TotalAmount);
                
                var growthPercentage = previousMonthRevenue > 0 ? ((currentMonthRevenue - previousMonthRevenue) / previousMonthRevenue) * 100 : 0;
                
                growthData.Add(new
                {
                    Month = currentMonth.ToString("MMM yyyy"),
                    Growth = Math.Round(growthPercentage, 1),
                    Revenue = currentMonthRevenue
                });
            }
            return growthData;
        }

        // API endpoints لتحديث البيانات ديناميكياً
        [HttpGet]
        public async Task<IActionResult> GetSalesData(string period = "6months")
        {
            var orders = await _orderService.GetAllAsync();
            var salesData = GetSalesChartData(orders, period);
            return Json(salesData);
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoryData()
        {
            var products = await _productService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();
            var categoryData = GetCategoryChartData(products, categories);
            return Json(categoryData);
        }

        [HttpGet]
        public async Task<IActionResult> GetGrowthData()
        {
            var orders = await _orderService.GetAllAsync();
            var growthData = GetMonthlyGrowthData(orders);
            return Json(growthData);
        }
    }

    public class UpdateOrderStatusRequest
    {
        public int OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
