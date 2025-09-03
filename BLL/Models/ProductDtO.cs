using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models
{
    internal class ProductDtO
    {
        public int Id { get; set; }             
        public string Name { get; set; } = null!;
        public string Brand { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = null!;
        public int Stock { get; set; }

        public int CategoryId { get; set; }
    }
}
