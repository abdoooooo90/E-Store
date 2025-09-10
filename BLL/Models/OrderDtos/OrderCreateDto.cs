using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models.OrderDtos
{
    public class OrderCreateDto
    {
        public string UserId { get; set; } = null!;
        public List<OrderItemCreateDto> Items { get; set; } = new();
        public ShippingAddressDto ShippingAddress { get; set; } = new();
        public string PaymentMethod { get; set; } = null!;
        public string? Notes { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class ShippingAddressDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string ZipCode { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string? Phone { get; set; }
    }
}
