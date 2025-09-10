using AutoMapper;
using BLL.Models.WishlistDtos;
using DAL.Models;

namespace BLL.MappingProfiles
{
    public class WishlistProfile : Profile
    {
        public WishlistProfile()
        {
            CreateMap<Wishlist, WishlistDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductBrand, opt => opt.MapFrom(src => ""))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
                .ForMember(dest => dest.ProductStock, opt => opt.MapFrom(src => src.Product.Stock));

            CreateMap<WishlistCreateDto, Wishlist>();
        }
    }
}
