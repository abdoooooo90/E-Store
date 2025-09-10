using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models.WishlistDtos
{
    public class WishlistDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductBrand { get; set; } = null!;
        public decimal ProductPrice { get; set; }
        public string ProductImageUrl { get; set; } = null!;
        public int ProductStock { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
