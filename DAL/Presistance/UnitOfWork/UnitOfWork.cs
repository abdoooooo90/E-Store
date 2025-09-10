using DAL.Presistance.Data;
using DAL.Presistance.Repositories.CartItems;
using DAL.Presistance.Repositories.Categories;
using DAL.Presistance.Repositories.Orders;
using DAL.Presistance.Repositories.Payments;
using DAL.Presistance.Repositories.ProductImages;
using DAL.Presistance.Repositories.Products;
using DAL.Presistance.Repositories.Wishlists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Presistance.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IProductRepository Products { get; }
        public ICategoryRepository Categories { get; }
        public IOrderRepository Orders { get; }
        public ICartItemRepository CartItems { get; }
        public IProductImagesRepository ProductImages { get; }
        public IPaymentRepository Payments { get; }
        public IWishlistRepository Wishlists { get; }

        public UnitOfWork(
            ApplicationDbContext context,
            IProductRepository productRepo,
            ICategoryRepository categoryRepo,
            IOrderRepository orderRepo,
            ICartItemRepository cartItemRepo,
            IProductImagesRepository productImageRepo,
            IPaymentRepository payments,
            IWishlistRepository wishlists)
        {
            _context = context;
            Products = productRepo;
            Categories = categoryRepo;
            Orders = orderRepo;
            CartItems = cartItemRepo;
            ProductImages = productImageRepo;
            Payments = payments;
            Wishlists = wishlists;
        }

        public async Task<int> CompleteAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                // في حالة Optimistic Concurrency Exception
                // نعيد 0 للإشارة إلى عدم تأثر أي صفوف
                return 0;
            }
            catch (Exception)
            {
                // في حالة أي خطأ آخر
                return 0;
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }

    }
}
