using BLL.Models.ProductDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models.CategoryDtos
{
    public class CategoryDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
    }
}

