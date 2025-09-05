using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models.ProductImageDtos
{
    public class ProductImageDetailsDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; } = null!;
        public ProductImageDto Product { get; set; } = null!;

    }
}
