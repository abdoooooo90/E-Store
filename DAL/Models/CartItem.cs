using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class CartItem : BaseEntity
    {
         
        public string UserId { get; set; } = null!;
        
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public ApplicationUser User { get; set; } = null!;
        public Product Product { get; set; } = null!;

      
    }
}
