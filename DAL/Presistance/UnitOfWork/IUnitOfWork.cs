using DAL.Presistance.Repositories.CartItems;
using DAL.Presistance.Repositories.Categories;
using DAL.Presistance.Repositories.Orders;
using DAL.Presistance.Repositories.Payments;
using DAL.Presistance.Repositories.Products;
using DAL.Presistance.Repositories.Wishlists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Presistance.UnitOfWork
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        IOrderRepository Orders { get; }
        ICartItemRepository CartItems { get; }
        IPaymentRepository Payments { get; }
        IWishlistRepository Wishlists { get; }
        Task<int> CompleteAsync();
    }
}
