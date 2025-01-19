using AutoMapper;
using TiendaOnline.Core.Entities;
using TiendaOnline.Application.DTOs;

namespace TiendaOnline.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeo entre ApplicationUser y UserDto
            CreateMap<ApplicationUser, UserDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

            // Mapeo entre Address y AddressDto
            CreateMap<Address, AddressDto>();

            // Mapeo entre Order y OrderDto
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.OrderProducts, opt => opt.MapFrom(src => src.OrderProducts));

            // Mapeo entre OrderProduct y OrderProductDto
            CreateMap<OrderProduct, OrderProductDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

            // Mapeo entre Product y ProductDto
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(i => i.ImageUrl).ToList()));

            CreateMap<ProductDto, Product>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Images, opt => opt.Ignore()); // Ignorar imágenes al mapear de DTO a entidad

            // Mapeo entre Cart y CartDto
            CreateMap<Cart, CartDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            // Mapeo entre CartItem y CartItemDto
            CreateMap<CartItem, CartItemDto>();
            CreateMap<CartItemDto, CartItem>();
        }
    }
}