using DAL.Presistance.Data;
using DAL.Presistance.Repositories.CartItems;
using DAL.Presistance.Repositories.Categories;
using DAL.Presistance.Repositories.Orders;
using DAL.Presistance.Repositories.Payments;
using DAL.Presistance.Repositories.ProductImages;
using DAL.Presistance.Repositories.Products;
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

        public UnitOfWork(
            ApplicationDbContext context,
            IProductRepository productRepo,
            ICategoryRepository categoryRepo,
            IOrderRepository orderRepo,
            ICartItemRepository cartItemRepo,
            IProductImagesRepository productImageRepo,
            IPaymentRepository payments)
        {
            _context = context;
            Products = productRepo;
            Categories = categoryRepo;
            Orders = orderRepo;
            CartItems = cartItemRepo;
            ProductImages = productImageRepo;
            Payments = payments;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }

    }
}
