using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models.Order
{
    public class OrderCreateDto
    {
        public string UserId { get; set; } = null!;
        public List<OrderItemCreateDto> Items { get; set; } = new();
    }
}
