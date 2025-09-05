using BLL.Models.PaymentDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.PaymentServices
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDto>> GetAllAsync();
        Task<PaymentDetailsDto?> GetByIdAsync(int id);
        Task<PaymentDto> CreateAsync(PaymentCreateDto dto);
        Task<bool> UpdateStatusAsync(PaymentUpdateStatusDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
