using DAL.Models;
using DAL.Presistance.Data;
using DAL.Presistance.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Presistance.Repositories.Orders
{
    public class OrderRepository: GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext dbcontext): base(dbcontext)
        {
            
        }
    }
}
