using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models
{
    internal class ProductImageWithProductDtO
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;

        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
    }
}
