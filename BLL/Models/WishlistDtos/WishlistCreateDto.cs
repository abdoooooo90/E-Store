using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models.WishlistDtos
{
    public class WishlistCreateDto
    {
        public string UserId { get; set; } = null!;
        public int ProductId { get; set; }
    }
}
