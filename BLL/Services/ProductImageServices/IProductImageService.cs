using BLL.Models.ProductImageDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.ProductImageServices
{
    public interface IProductImageService
    {
        Task<IEnumerable<ProductImageDto>> GetAllAsync();
        Task<ProductImageDetailsDto?> GetByIdAsync(int id);
        Task<ProductImageDto> CreateAsync(ProductImageCreateDto dto);
        Task<bool> UpdateAsync(int id, ProductImageUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
