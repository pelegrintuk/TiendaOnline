using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TiendaOnline.DAL.Data;

namespace TiendaOnline.DAL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDAL(this IServiceCollection services, IConfiguration configuration)
        {
            // Registrar el contexto de datos principal (TiendaContext)
            services.AddDbContext<TiendaContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("TiendaOnline.DAL")
                ));

            // Registrar el contexto de datos de autenticación (ApplicationDbContext)
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("TiendaOnline.DAL")
                ));

            return services;
        }
    }
}
