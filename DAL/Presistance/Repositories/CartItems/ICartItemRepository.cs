using DAL.Models;
using DAL.Presistance.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Presistance.Repositories.CartItems
{
    public interface ICartItemRepository: IGenericRepository<CartItem>
    {
        Task<IEnumerable<CartItem>> GetByUserIdAsync(string userId);
        Task<CartItem?> GetByUserIdAndProductIdAsync(string userId, int productId);
        Task<int> GetCountByUserIdAsync(string userId);
    }
}
