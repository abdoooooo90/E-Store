using BLL.MappingProfiles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DAL.Presistance.Data;
using DAL.Models;
using BLL.Services.ProductServices;
using BLL.Services.CategorieServices;
using BLL.Services.CartItemServices;
using BLL.Services.WishlistServices;
using BLL.Services.OrderServices;
using BLL.Services.EmailServices;
using DAL.Presistance.Repositories.Products;
using DAL.Presistance.Repositories.Categories;
using DAL.Presistance.Repositories.CartItems;
using DAL.Presistance.Repositories.Wishlists;
using DAL.Presistance.Repositories.Orders;
using DAL.Presistance.Repositories.Payments;
using DAL.Presistance.UnitOfWork;
using System.Globalization;

namespace E_LapShop
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
            builder.Services.AddDbContext<ApplicationDbContext>((OptionsBuilder =>
            {
                OptionsBuilder.UseSqlServer((builder.Configuration.GetConnectionString("DefaultConnection")));
            }));

            builder.Services.AddAutoMapper(typeof(ProductProfile).Assembly);

          
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
            builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

           
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<ICartItemService, CartItemService>();
            builder.Services.AddScoped<IWishlistService, WishlistService>();
            builder.Services.AddScoped<IEmailService, EmailService>();

            // Add in-memory cache and session for storing coupon/discount info
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromHours(2);
            });

           
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Furni/Error");
                
                app.UseHsts();
            }

            
            var supportedCultures = new[] { "ar-EG", "en-US" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture("ar-EG")
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
            
            app.UseRequestLocalization(localizationOptions);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            // Enable session before authentication/authorization
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Furni}/{action=Index}/{id?}");

            // Seed Roles & Admin
            await SeedDataAsync(app);

            app.Run();
        }
        private static async Task SeedDataAsync(WebApplication app)
        {
            try
            {
                using (var scope = app.Services.CreateScope())
                {
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                    string[] roles = { "Admin", "User" };
                    foreach (var role in roles)
                    {
                        if (!await roleManager.RoleExistsAsync(role))
                            await roleManager.CreateAsync(new IdentityRole(role));
                    }

                    // Create Admin User
                    var adminEmail = "AdminMode@gmail.com";
                    var admin = await userManager.FindByEmailAsync(adminEmail);
                    if (admin == null)
                    {
                        admin = new ApplicationUser
                        {
                            UserName = adminEmail,
                            Email = adminEmail,
                            FullName = "مدير النظام",
                            EmailConfirmed = true
                        };

                        var result = await userManager.CreateAsync(admin, "Admin@123");
                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(admin, "Admin");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SeedDataAsync: {ex.Message}");
            }
        }
    }
}
