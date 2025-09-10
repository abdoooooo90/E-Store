using DAL.Models;
using DAL.Presistance.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Presistance.Repositories.Wishlists
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly ApplicationDbContext _context;

        public WishlistRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Wishlist>> GetByUserIdAsync(string userId)
        {
            return await _context.Wishlists
                .Include(w => w.Product)
                .ThenInclude(p => p.Category)
                .Where(w => w.UserId == userId)
                .ToListAsync();
        }

        public async Task<Wishlist?> GetByUserIdAndProductIdAsync(string userId, int productId)
        {
            return await _context.Wishlists
                .Include(w => w.Product)
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
        }

        public async Task<Wishlist> AddAsync(Wishlist wishlist)
        {
            await _context.Wishlists.AddAsync(wishlist);
            return wishlist;
        }

        public async Task DeleteAsync(Wishlist wishlist)
        {
            _context.Wishlists.Remove(wishlist);
            await Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(string userId, int productId)
        {
            return await _context.Wishlists
                .AnyAsync(w => w.UserId == userId && w.ProductId == productId);
        }

        public async Task<int> GetCountByUserIdAsync(string userId)
        {
            return await _context.Wishlists
                .CountAsync(w => w.UserId == userId);
        }
    }
}
