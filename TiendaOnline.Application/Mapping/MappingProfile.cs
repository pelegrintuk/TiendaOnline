using AutoMapper;
using TiendaOnline.Application.DTOs;
using TiendaOnline.Core.Entities;

namespace TiendaOnline.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserDto, ApplicationUser>().ReverseMap();
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.FirstOrDefault().ImageUrl))
                .ReverseMap();
            CreateMap<OrderDto, Order>().ReverseMap();
            CreateMap<CartDto, Cart>().ReverseMap();
        }
    }
}
