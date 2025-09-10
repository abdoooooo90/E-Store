using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Presistance.Repositories.Wishlists
{
    public interface IWishlistRepository
    {
        Task<IEnumerable<Wishlist>> GetByUserIdAsync(string userId);
        Task<Wishlist?> GetByUserIdAndProductIdAsync(string userId, int productId);
        Task<Wishlist> AddAsync(Wishlist wishlist);
        Task DeleteAsync(Wishlist wishlist);
        Task<bool> ExistsAsync(string userId, int productId);
        Task<int> GetCountByUserIdAsync(string userId);
    }
}
