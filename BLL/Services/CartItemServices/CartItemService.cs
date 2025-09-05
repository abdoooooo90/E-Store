using AutoMapper;
using BLL.Models.CartItemDtos;
using DAL.Models;
using DAL.Presistance.UnitOfWork;
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

        public async Task<CartItemDto?> GetByIdAsync(int id)
        {
            var item = await _unitOfWork.CartItems.GetByIdAsync(id);
            return item == null ? null : _mapper.Map<CartItemDto>(item);
        }

        public async Task<CartItemDto> CreateAsync(CartItemDto dto)
        {
            var item = _mapper.Map<CartItem>(dto);
            _unitOfWork.CartItems.Add(item);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<CartItemDto>(item);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _unitOfWork.CartItems.GetByIdAsync(id);
            if (item == null) return false;

            _unitOfWork.CartItems.Delete(item);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
