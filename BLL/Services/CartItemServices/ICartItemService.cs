using BLL.Models.CartItemDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.CartItemServices
{
    public interface ICartItemService
    {
        Task<IEnumerable<CartItemDto>> GetAllAsync();
        Task<CartItemDto?> GetByIdAsync(int id);
        Task<CartItemDto> CreateAsync(CartItemDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
