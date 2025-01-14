using Microsoft.Extensions.DependencyInjection;
using TiendaOnline.Application.Interfaces;
using TiendaOnline.Infrastructure.Logging;

namespace TiendaOnline.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<ILogService, LogService>();
            return services;
        }
    }
}
