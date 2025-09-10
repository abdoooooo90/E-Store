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
        Task<IEnumerable<CartItemDto>> GetUserCartAsync(string userId);
        Task<CartItemDto?> GetByIdAsync(int id);
        Task<CartItemDto> CreateAsync(CartItemDto dto);
        Task<bool> AddToCartAsync(CartItemCreateDto dto);
        Task<bool> UpdateQuantityAsync(int cartItemId, int quantity);
        Task<bool> DeleteAsync(int id);
        Task<bool> ClearUserCartAsync(string userId);
        Task<int> GetCartCountAsync(string userId);
        Task<decimal> GetCartTotalAsync(string userId);
    }
}
