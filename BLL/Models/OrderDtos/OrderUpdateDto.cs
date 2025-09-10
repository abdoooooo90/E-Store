using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models.OrderDtos
{
    public class OrderUpdateDto
    {
        public int Id { get; set; }
        public List<OrderItemCreateDto> Items { get; set; } = new();
        public string Status { get; set; } = null!;

    }
}
