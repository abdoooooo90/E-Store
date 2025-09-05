using BLL.Models.CartItemDtos;
using BLL.Models.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models.UserDtos
{
    public class UserDetailsDto
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;

        public List<OrderDto> Orders { get; set; } = new List<OrderDto>();
        public List<CartItemDto> CartItems { get; set; } = new List<CartItemDto>();
    }
}
