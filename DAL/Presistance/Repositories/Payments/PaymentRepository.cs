using DAL.Models;
using DAL.Presistance.Data;
using DAL.Presistance.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Presistance.Repositories.Payments
{
    public class PaymentRepository: GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext dbcontext): base(dbcontext)
        {
            
        }
    }
}
