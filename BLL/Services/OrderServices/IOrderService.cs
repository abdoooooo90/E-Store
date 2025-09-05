using BLL.Models.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.OrderServices
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllAsync();
        Task<OrderDetailsDto?> GetByIdAsync(int id);
        Task<OrderDto> CreateAsync(OrderCreateDto dto);
        Task<bool> UpdateAsync(OrderUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
