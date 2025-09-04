using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models.CartItemDtos
{
    public class CartDto
    {
        public string UserId { get; set; } = null!;
        public List<CartItemDto> Items { get; set; } = new();
        public decimal TotalPrice => Items.Sum(i => i.Total);
    }
}
