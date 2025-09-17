using AutoMapper;
using BLL.Models.WishlistDtos;
using BLL.Models.CartItemDtos;
using BLL.Services.CartItemServices;
using DAL.Models;
using DAL.Presistance.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.WishlistServices
{
    public class WishlistService : IWishlistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICartItemService _cartItemService;

        public WishlistService(IUnitOfWork unitOfWork, IMapper mapper, ICartItemService cartItemService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cartItemService = cartItemService;
        }

        public async Task<IEnumerable<WishlistDto>> GetUserWishlistAsync(string userId)
        {
            var wishlists = await _unitOfWork.Wishlists.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<WishlistDto>>(wishlists);
        }

        public async Task<bool> AddToWishlistAsync(WishlistCreateDto dto)
        {
            try
            {
                // Check if item already exists in wishlist
                var exists = await _unitOfWork.Wishlists.ExistsAsync(dto.UserId, dto.ProductId);

                if (exists)
                    return false; // Already in wishlist

                var wishlistItem = _mapper.Map<Wishlist>(dto);
                await _unitOfWork.Wishlists.AddAsync(wishlistItem);
                
                return await _unitOfWork.CompleteAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveFromWishlistAsync(string userId, int productId)
        {
            try
            {
                var wishlistItem = await _unitOfWork.Wishlists.GetByUserIdAndProductIdAsync(userId, productId);
                if (wishlistItem == null)
                    return false;

                await _unitOfWork.Wishlists.DeleteAsync(wishlistItem);
                return await _unitOfWork.CompleteAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsInWishlistAsync(string userId, int productId)
        {
            var existingItem = await _unitOfWork.Wishlists.GetByUserIdAndProductIdAsync(userId, productId);
            
            return existingItem != null;
        }

        public async Task<int> GetWishlistCountAsync(string userId)
        {
            return await _unitOfWork.Wishlists.GetCountByUserIdAsync(userId);
        }

        public Task<bool> MoveToCartAsync(int wishlistId, int quantity = 1)
        {
            try
            {
                // This method needs to be redesigned to work with the repository pattern
                // For now, return false as it requires additional context
                return Task.FromResult(false);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }
    }
}
