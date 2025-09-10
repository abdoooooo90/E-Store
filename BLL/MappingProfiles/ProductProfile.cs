using AutoMapper;
using BLL.Models.ProductDtos;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.MappingProfiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.Stock))
                .ReverseMap()
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.StockQuantity));

            CreateMap<Product, ProductCreateDto>()
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.Stock))
                .ReverseMap()
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.StockQuantity));

            CreateMap<Product, ProductUpdateDto>()
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.Stock))
                .ReverseMap()
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.StockQuantity));

            CreateMap<Product, ProductDetailsDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(img => img.ImageUrl)))
                .ForMember(dest => dest.TotalOrders, opt => opt.MapFrom(src => src.OrderItems.Count))
                .ForMember(dest => dest.TotalInCarts, opt => opt.MapFrom(src => src.CartItems.Count))
                .ReverseMap()
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.Ignore());
        }
    }
}
