using DAL.Models;
using DAL.Presistance.Data;
using DAL.Presistance.Repositories.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Presistance.Repositories.CartItems
{
    public class CartItemRepository: GenericRepository<CartItem>, ICartItemRepository
    {
        public CartItemRepository(ApplicationDbContext dbcontext): base(dbcontext)
        {
            
        }

        public async Task<IEnumerable<CartItem>> GetByUserIdAsync(string userId)
        {
            return await _context.CartItems
                .Include(c => c.Product)
                .ThenInclude(p => p.Category)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<CartItem?> GetByUserIdAndProductIdAsync(string userId, int productId)
        {
            return await _context.CartItems
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
        }

        public async Task<int> GetCountByUserIdAsync(string userId)
        {
            return await _context.CartItems
                .CountAsync(c => c.UserId == userId);
        }
    }
}
