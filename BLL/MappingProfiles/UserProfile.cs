using AutoMapper;
using BLL.Models.UserDtos;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.MappingProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserDto>().ReverseMap();
            CreateMap<ApplicationUser, UserCreateDto>().ReverseMap();
            CreateMap<ApplicationUser, UserUpdateDto>().ReverseMap();

            CreateMap<ApplicationUser, UserDetailsDto>()
                .ForMember(dest => dest.Orders, opt => opt.MapFrom(src => src.Orders))
                .ForMember(dest => dest.CartItems, opt => opt.MapFrom(src => src.CartItems))
                .ReverseMap()
                .ForMember(dest => dest.Orders, opt => opt.Ignore())
                .ForMember(dest => dest.CartItems, opt => opt.Ignore());
        }
    }
}
