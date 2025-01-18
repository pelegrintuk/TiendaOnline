using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace TiendaOnline.DAL.Factories
{
    public class TiendaContextFactory : IDesignTimeDbContextFactory<TiendaContext>
    {
        public TiendaContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TiendaContext>();

            // Leer la configuración desde el archivo appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);

            return new TiendaContext(optionsBuilder.Options);
        }
    }
}
