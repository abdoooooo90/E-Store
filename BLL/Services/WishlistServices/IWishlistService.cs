using BLL.Models.WishlistDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.WishlistServices
{
    public interface IWishlistService
    {
        Task<IEnumerable<WishlistDto>> GetUserWishlistAsync(string userId);
        Task<bool> AddToWishlistAsync(WishlistCreateDto dto);
        Task<bool> RemoveFromWishlistAsync(string userId, int productId);
        Task<bool> IsInWishlistAsync(string userId, int productId);
        Task<int> GetWishlistCountAsync(string userId);
        Task<bool> MoveToCartAsync(int wishlistId, int quantity = 1);
    }
}
