using Microsoft.AspNetCore.Identity;
using TiendaOnline.Application;
using TiendaOnline.Core.Entities;
using TiendaOnline.DAL;
using TiendaOnline.Infrastructure;
using TiendaOnline.Infrastructure.DependencyInjection;
using TiendaOnline.Services.Filters;

var builder = WebApplication.CreateBuilder(args);

// Registrar dependencias de Application, DAL e Infrastructure
builder.Services.AddApplication();
builder.Services.AddDAL(builder.Configuration);
builder.Services.AddInfrastructure();

// Configurar Identity para autenticaci�n
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<TiendaContext>()
    .AddDefaultTokenProviders(); // Proveedores de tokens para autenticaci�n y recuperaci�n de contrase�as

// Configurar controladores y filtros globales
builder.Services.AddControllers(options =>
{
    // A�adir filtros personalizados
    options.Filters.Add<CustomExceptionFilter>();
    options.Filters.Add<ValidationFilter>();
});

// Configurar Swagger para documentar la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//A�adimos AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configuraci�n de entorno de desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TiendaOnline API v1");
        c.RoutePrefix = string.Empty; // Swagger en la ra�z
    });
}

// Configuraci�n del middleware
app.UseHttpsRedirection();
app.UseAuthentication(); // Middleware para autenticaci�n
app.UseAuthorization();  // Middleware para autorizaci�n

// Mapear controladores
app.MapControllers();

// Ejecutar la aplicaci�n
app.Run();
