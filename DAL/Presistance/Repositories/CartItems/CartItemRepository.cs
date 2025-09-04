using DAL.Models;
using DAL.Presistance.Data;
using DAL.Presistance.Repositories.Generic;
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
    }
}
