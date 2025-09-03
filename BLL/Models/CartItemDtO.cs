using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models
{
    internal class CartItemDTO
    {
        public int Id { get; set; }        
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

       
        // public string UserId { get; set; } = null!;
        // public string ProductName { get; set; } = null!;
        // public decimal ProductPrice { get; set; }
    }
}
