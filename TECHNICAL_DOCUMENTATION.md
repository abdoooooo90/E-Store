# الوثائق التقنية - Furni Store

## 🏗️ البنية المعمارية

### نمط 3-Layer Architecture

```
┌─────────────────────────────────────┐
│         Presentation Layer (PL)     │  ← Controllers, Views, Models
├─────────────────────────────────────┤
│         Business Logic Layer (BLL)  │  ← Services, DTOs, Profiles
├─────────────────────────────────────┤
│         Data Access Layer (DAL)     │  ← Repositories, Models, DbContext
└─────────────────────────────────────┘
```

### تفاصيل الطبقات

#### Presentation Layer (E-LapShop)
- **Controllers**: معالجة طلبات HTTP
- **Views**: واجهات المستخدم (Razor Pages)
- **Models**: View Models و DTOs
- **wwwroot**: الملفات الثابتة (CSS, JS, Images)

#### Business Logic Layer (BLL)
- **Services**: منطق الأعمال الأساسي
- **DTOs**: كائنات نقل البيانات
- **Profiles**: تكوين AutoMapper
- **Validation**: قواعد التحقق من البيانات

#### Data Access Layer (DAL)
- **Models**: Entity Models
- **Data**: DbContext و Configurations
- **Repositories**: تنفيذ Repository Pattern
- **Migrations**: تحديثات قاعدة البيانات

## 🗄️ قاعدة البيانات

### مخطط قاعدة البيانات

```sql
-- المستخدمون (ASP.NET Identity)
AspNetUsers
├── Id (PK)
├── FullName
├── Email
├── Address
└── ...

-- المنتجات
Products
├── Id (PK)
├── Name
├── Description
├── Price
├── Stock
├── ImageUrl
├── CategoryId (FK)
└── CreatedAt

-- الفئات
Categories
├── Id (PK)
├── Name
└── Description

-- عربة التسوق
CartItems
├── Id (PK)
├── UserId (FK)
├── ProductId (FK)
├── Quantity
└── CreatedAt

-- قائمة الأمنيات
Wishlists
├── Id (PK)
├── UserId (FK)
├── ProductId (FK)
└── CreatedAt

-- الطلبات
Orders
├── Id (PK)
├── UserId (FK)
├── TotalAmount
├── Status
├── ShippingAddress
├── PaymentMethod
└── CreatedAt

-- تفاصيل الطلبات
OrderItems
├── Id (PK)
├── OrderId (FK)
├── ProductId (FK)
├── Quantity
├── Price
└── Total
```

### العلاقات
- `Categories` → `Products` (One-to-Many)
- `Users` → `CartItems` (One-to-Many)
- `Users` → `Wishlists` (One-to-Many)
- `Users` → `Orders` (One-to-Many)
- `Products` → `CartItems` (One-to-Many)
- `Products` → `Wishlists` (One-to-Many)
- `Orders` → `OrderItems` (One-to-Many)

## 🔧 الخدمات والمستودعات

### Services Structure

```csharp
// Interface
public interface IProductService
{
    Task<IEnumerable<ProductReadDto>> GetAllAsync();
    Task<ProductReadDto> GetByIdAsync(int id);
    Task<ProductReadDto> CreateAsync(ProductCreateDto dto);
    Task<ProductReadDto> UpdateAsync(int id, ProductUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}

// Implementation
public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    
    public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    // Methods implementation...
}
```

### Repository Pattern

```csharp
// Generic Repository Interface
public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    Task<T> AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}

// Specific Repository
public interface IProductRepository : IGenericRepository<Product>
{
    Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);
    Task<IEnumerable<Product>> SearchAsync(string searchTerm);
}
```

### Unit of Work Pattern

```csharp
public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }
    ICartItemRepository CartItems { get; }
    IWishlistRepository Wishlists { get; }
    IOrderRepository Orders { get; }
    
    Task<int> CompleteAsync();
}
```

## 🔐 الأمان والمصادقة

### ASP.NET Core Identity Configuration

```csharp
// Program.cs
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    
    // User settings
    options.User.RequireUniqueEmail = true;
    
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();
```

### Authorization Policies

```csharp
// Controllers
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    // Admin-only actions
}

[Authorize]
public class CartController : Controller
{
    // Authenticated users only
}
```

### CSRF Protection

```html
<!-- في النماذج -->
@Html.AntiForgeryToken()

<!-- في AJAX -->
headers: {
    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
}
```

## 🎨 Frontend Architecture

### CSS Organization

```
wwwroot/css/
├── site.css           # الأنماط العامة
├── furni.css          # أنماط متجر الأثاث
├── admin.css          # أنماط لوحة التحكم
└── dark-mode.css      # أنماط الوضع الليلي
```

### JavaScript Structure

```javascript
// Site-wide functionality
const FurniStore = {
    // Cart functionality
    cart: {
        add: async (productId, quantity) => { /* ... */ },
        remove: async (productId) => { /* ... */ },
        update: async (productId, quantity) => { /* ... */ }
    },
    
    // Wishlist functionality
    wishlist: {
        add: async (productId) => { /* ... */ },
        remove: async (productId) => { /* ... */ }
    },
    
    // UI utilities
    ui: {
        showToast: (message, type) => { /* ... */ },
        updateCounters: () => { /* ... */ },
        toggleDarkMode: () => { /* ... */ }
    }
};
```

## 📡 API Endpoints

### Product Endpoints
```
GET    /api/products              # جميع المنتجات
GET    /api/products/{id}         # منتج محدد
POST   /api/products              # إضافة منتج جديد
PUT    /api/products/{id}         # تحديث منتج
DELETE /api/products/{id}         # حذف منتج
```

### Cart Endpoints
```
GET    /Cart                      # عرض العربة
POST   /Cart/Add                  # إضافة للعربة
POST   /Cart/Update               # تحديث الكمية
POST   /Cart/Remove               # حذف من العربة
POST   /Cart/ApplyCoupon          # تطبيق كوبون
```

### Wishlist Endpoints
```
GET    /Wishlist                  # عرض قائمة الأمنيات
POST   /Wishlist/AddToWishlist    # إضافة للأمنيات
POST   /Wishlist/RemoveFromWishlist # حذف من الأمنيات
GET    /Wishlist/IsInWishlist     # فحص وجود المنتج
GET    /Wishlist/GetWishlistCount # عدد عناصر الأمنيات
```

## 🔄 State Management

### TempData Usage
```csharp
// في Controller
TempData["WelcomeMessage"] = "مرحباً بك!";
TempData["ErrorMessage"] = "حدث خطأ!";

// في View
@if (TempData["WelcomeMessage"] != null)
{
    <div class="alert alert-success">
        @TempData["WelcomeMessage"]
    </div>
}
```

### Session Management
```csharp
// حفظ في Session
HttpContext.Session.SetString("CartId", cartId);

// استرجاع من Session
var cartId = HttpContext.Session.GetString("CartId");
```

## 🧪 الاختبارات

### Unit Tests Structure
```csharp
[TestClass]
public class ProductServiceTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IMapper> _mockMapper;
    private ProductService _productService;
    
    [TestInitialize]
    public void Setup()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _productService = new ProductService(_mockUnitOfWork.Object, _mockMapper.Object);
    }
    
    [TestMethod]
    public async Task GetAllAsync_ReturnsAllProducts()
    {
        // Arrange
        var products = new List<Product> { /* test data */ };
        _mockUnitOfWork.Setup(u => u.Products.GetAllAsync())
                      .ReturnsAsync(products);
        
        // Act
        var result = await _productService.GetAllAsync();
        
        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(products.Count, result.Count());
    }
}
```

## 📊 الأداء والتحسين

### Database Optimization
```csharp
// Indexing
modelBuilder.Entity<Product>()
    .HasIndex(p => p.Name)
    .HasDatabaseName("IX_Product_Name");

// Eager Loading
var products = await _context.Products
    .Include(p => p.Category)
    .ToListAsync();

// Pagination
var products = await _context.Products
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

### Caching Strategy
```csharp
// Memory Cache
services.AddMemoryCache();

// في Service
public async Task<IEnumerable<ProductReadDto>> GetAllAsync()
{
    const string cacheKey = "all_products";
    
    if (!_cache.TryGetValue(cacheKey, out IEnumerable<ProductReadDto> products))
    {
        products = await GetProductsFromDatabase();
        _cache.Set(cacheKey, products, TimeSpan.FromMinutes(30));
    }
    
    return products;
}
```

## 🚀 النشر والتوزيع

### Production Configuration
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-server;Database=FurniStore;..."
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Docker Configuration
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["E-LapShop/E-Store.PL.csproj", "E-LapShop/"]
COPY ["BLL/BLL.csproj", "BLL/"]
COPY ["DAL/DAL.csproj", "DAL/"]
RUN dotnet restore "E-LapShop/E-Store.PL.csproj"

COPY . .
WORKDIR "/src/E-LapShop"
RUN dotnet build "E-Store.PL.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "E-Store.PL.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "E-Store.PL.dll"]
```

## 🔍 المراقبة والسجلات

### Logging Configuration
```csharp
// Program.cs
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();

// في Controllers
private readonly ILogger<ProductController> _logger;

public ProductController(ILogger<ProductController> logger)
{
    _logger = logger;
}

public async Task<IActionResult> Index()
{
    _logger.LogInformation("Accessing products index page");
    // ...
}
```

### Error Handling
```csharp
// Global Exception Handler
app.UseExceptionHandler("/Home/Error");

// في Controller
try
{
    // Business logic
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error occurred while processing request");
    return View("Error");
}
```

## 📱 الاستجابة والتوافق

### Responsive Design
```css
/* Mobile First Approach */
.product-card {
    width: 100%;
    margin-bottom: 1rem;
}

@media (min-width: 768px) {
    .product-card {
        width: 48%;
    }
}

@media (min-width: 1024px) {
    .product-card {
        width: 31%;
    }
}
```

### Browser Compatibility
- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

## 🔧 أدوات التطوير

### Development Tools
- **Visual Studio 2022**: IDE الرئيسي
- **SQL Server Management Studio**: إدارة قاعدة البيانات
- **Postman**: اختبار APIs
- **Git**: نظام التحكم في الإصدارات

### NuGet Packages
```xml
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation" Version="8.0.0" />
```

---

هذه الوثائق التقنية توفر فهماً شاملاً لبنية وتقنيات مشروع متجر الأثاث.
