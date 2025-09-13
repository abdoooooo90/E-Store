using AutoMapper;
using BLL.Models.OrderDtos;
using DAL.Models;
using DAL.Presistance.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.OrderServices
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            var orders = await _unitOfWork.Orders.GetAllAsync();
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDetailsDto?> GetByIdAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            return order == null ? null : _mapper.Map<OrderDetailsDto>(order);
        }

        public async Task<OrderDto> CreateAsync(OrderCreateDto dto)
        {
            var order = _mapper.Map<Order>(dto);
            
            // Set default values
            order.OrderDate = DateTime.Now;
            order.Status = "Pending";
            
            // Update stock quantities for each item in the order
            foreach (var item in dto.Items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product != null && product.Stock >= item.Quantity)
                {
                    product.Stock -= item.Quantity;
                    _unitOfWork.Products.Update(product);
                }
                else
                {
                    // Not enough stock - return null to indicate failure
                    return null;
                }
            }
            
            _unitOfWork.Orders.Add(order);
            var result = await _unitOfWork.CompleteAsync();
            
            if (result > 0)
            {
                return _mapper.Map<OrderDto>(order);
            }
            
            return null;
        }

        public async Task<bool> UpdateAsync(OrderUpdateDto dto)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(dto.Id);
            if (order == null) return false;

            _mapper.Map(dto, order);
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null) return false;

            _unitOfWork.Orders.Delete(order);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
