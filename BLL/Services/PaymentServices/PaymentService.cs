using AutoMapper;
using BLL.Models.PaymentDtos;
using DAL.Models;
using DAL.Presistance.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.PaymentServices
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PaymentDto>> GetAllAsync()
        {
            var payments = await _unitOfWork.Payments.GetAllAsync();
            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<PaymentDetailsDto?> GetByIdAsync(int id)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(id);
            return payment == null ? null : _mapper.Map<PaymentDetailsDto>(payment);
        }

        public async Task<PaymentDto> CreateAsync(PaymentCreateDto dto)
        {
            var payment = _mapper.Map<Payment>(dto);
            _unitOfWork.Payments.Add(payment);
            await _unitOfWork.CompleteAsync(); 
            return _mapper.Map<PaymentDto>(payment);
        }

        public async Task<bool> UpdateStatusAsync(PaymentUpdateStatusDto dto)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(dto.PaymentId);
            if (payment == null) return false;

            payment.Status = dto.Status; 
            _unitOfWork.Payments.Update(payment);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(id);
            if (payment == null) return false;

            _unitOfWork.Payments.Delete(payment);
            await _unitOfWork.CompleteAsync(); 
            return true;
        }
    }
}
