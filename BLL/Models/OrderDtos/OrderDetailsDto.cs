using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models.OrderDtos
{
    public class OrderDetailsDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string UserFullName { get; set; } = null!;
        public string ShippingAddress { get; set; } = null!;
        public string? Notes { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();
    }
}
