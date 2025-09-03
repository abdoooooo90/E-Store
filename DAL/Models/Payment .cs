using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Payment : BaseEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
