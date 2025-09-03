using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models
{
    internal class ApplicationUserDTO
    {

        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string FullName { get; set; } = null!;

        // public List<OrderDto> Orders { get; set; } = new();
        // public List<CartItemDto> CartItems { get; set; } = new();
    }
}
