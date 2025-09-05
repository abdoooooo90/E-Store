using AutoMapper;
using BLL.Models.PaymentDtos;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.MappingProfiles
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<Payment, PaymentDto>().ReverseMap();
            CreateMap<Payment, PaymentCreateDto>().ReverseMap();
            CreateMap<Payment, PaymentUpdateStatusDto>().ReverseMap();

            CreateMap<Payment, PaymentDetailsDto>()
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.Order.Status))
                .ForMember(dest => dest.OrderTotal, opt => opt.MapFrom(src => src.Order.TotalAmount))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt)) // لو عندك CreatedAt في BaseEntity
                .ReverseMap();
        }
    }
}
