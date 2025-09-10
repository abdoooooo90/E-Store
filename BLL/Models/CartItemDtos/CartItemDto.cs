using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models.CartItemDtos
{
    public class CartItemDto
    {
        public int Id { get; set; }    
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductBrand { get; set; } = null!;
        public string ProductImageUrl { get; set; } = null!;
        public decimal ProductPrice { get; set; }
        public int ProductStock { get; set; }
        public int Quantity { get; set; }
        public decimal Total => ProductPrice * Quantity;
    }
}
