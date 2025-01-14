using Microsoft.Extensions.DependencyInjection;
using TiendaOnline.Application.Interfaces;
using TiendaOnline.Application.Services;

namespace TiendaOnline.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Registrar servicios de lógica de negocio
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICartService, CartService>();

            return services;
        }
    }
}

