using AutoMapper;
using BLL.Models.CartItemDtos;
using DAL.Models;
using DAL.Presistance.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.CartItemServices
{
    public class CartItemService : ICartItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CartItemService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CartItemDto>> GetAllAsync()
        {
            var items = await _unitOfWork.CartItems.GetAllAsync();
            return _mapper.Map<IEnumerable<CartItemDto>>(items);
        }

        public async Task<IEnumerable<CartItemDto>> GetUserCartAsync(string userId)
        {
            var items = await _unitOfWork.CartItems.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<CartItemDto>>(items);
        }

        public async Task<CartItemDto?> GetByIdAsync(int id)
        {
            var item = await _unitOfWork.CartItems.GetByIdAsync(id);
            return item == null ? null : _mapper.Map<CartItemDto>(item);
        }

        public async Task<CartItemDto> CreateAsync(CartItemDto dto)
        {
            var cartItem = _mapper.Map<CartItem>(dto);
            _unitOfWork.CartItems.Add(cartItem);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<CartItemDto>(cartItem);
        }

        public async Task<bool> AddToCartAsync(CartItemCreateDto dto)
        {
            try
            {
                // Get product to check stock and availability
                var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
                if (product == null)
                {
                    throw new Exception($"Product with ID {dto.ProductId} not found");
                }

                if (!product.IsAvailable)
                {
                    throw new Exception($"Product {product.Name} is not available");
                }

                if (product.Stock < dto.Quantity)
                {
                    throw new Exception($"Not enough stock for product {product.Name}. Available: {product.Stock}, Requested: {dto.Quantity}");
                }

                // Check if item already exists in cart
                var existingCartItem = await _unitOfWork.CartItems
                    .GetByUserIdAndProductIdAsync(dto.UserId, dto.ProductId);

                if (existingCartItem != null)
                {
                    // Check if total quantity would exceed stock
                    var totalQuantity = existingCartItem.Quantity + dto.Quantity;
                    if (product.Stock < totalQuantity)
                    {
                        throw new Exception($"Not enough stock for product {product.Name}. Available: {product.Stock}, Total requested: {totalQuantity}");
                    }
                    
                    // Update quantity
                    existingCartItem.Quantity += dto.Quantity;
                    _unitOfWork.CartItems.Update(existingCartItem);
                }
                else
                {
                    // Create new cart item
                    var newItem = new CartItem
                    {
                        UserId = dto.UserId,
                        ProductId = dto.ProductId,
                        Quantity = dto.Quantity,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _unitOfWork.CartItems.Add(newItem);
                }

                // Save changes
                var result = await _unitOfWork.CompleteAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                // Log the actual exception for debugging
                Console.WriteLine($"AddToCartAsync Error: {ex.Message}");
                throw; // Re-throw to let controller handle it
            }
        }

        public async Task<bool> UpdateQuantityAsync(int cartItemId, int quantity)
        {
            try
            {
                var cartItem = await _unitOfWork.CartItems.GetByIdAsync(cartItemId);
                if (cartItem == null) return false;

                if (quantity <= 0)
                {
                    _unitOfWork.CartItems.Delete(cartItem);
                }
                else
                {
                    // Get current product to check stock
                    var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId);
                    if (product == null)
                    {
                        throw new Exception("Product not found");
                    }

                    // Debug logging
                    Console.WriteLine($"UpdateQuantityAsync - Product: {product.Name}, Current Stock: {product.Stock}, Requested Quantity: {quantity}");

                    // Check if requested quantity exceeds available stock
                    if (quantity > product.Stock)
                    {
                        throw new Exception($"Cannot add more items. Only {product.Stock} items available in stock.");
                    }

                    cartItem.Quantity = quantity;
                    cartItem.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.CartItems.Update(cartItem);
                }

                return await _unitOfWork.CompleteAsync() > 0;
            }
            catch (Exception)
            {
                // Re-throw the exception so the controller can handle it properly
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var item = await _unitOfWork.CartItems.GetByIdAsync(id);
                if (item == null) return false;

                _unitOfWork.CartItems.Delete(item);
                return await _unitOfWork.CompleteAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ClearUserCartAsync(string userId)
        {
            try
            {
                var cartItems = await _unitOfWork.CartItems
                .GetByUserIdAsync(userId);

                foreach (var item in cartItems)
                {
                    _unitOfWork.CartItems.Delete(item);
                }

                return await _unitOfWork.CompleteAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> GetCartCountAsync(string userId)
        {
            return await _unitOfWork.CartItems
                .GetCountByUserIdAsync(userId);
        }

        public async Task<decimal> GetCartTotalAsync(string userId)
        {
            var items = await _unitOfWork.CartItems.GetByUserIdAsync(userId);
            return items.Sum(c => c.Product.Price * c.Quantity);
        }
    }
}
