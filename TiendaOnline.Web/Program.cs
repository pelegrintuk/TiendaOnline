using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.DAL;
using TiendaOnline.Core.Entities;
using TiendaOnline.Application;
using TiendaOnline.Infrastructure;
using TiendaOnline.Infrastructure.DependencyInjection;
using AutoMapper;
using TiendaOnline.Application.Mapping;
using TiendaOnline.Application.Interfaces;
using TiendaOnline.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar la cadena de conexión desde el archivo de configuración
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Configurar el contexto de datos con EF Core desde TiendaOnline.DAL
builder.Services.AddDAL(builder.Configuration);

// Configurar Identity con la clase personalizada ApplicationUser
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Deshabilitar confirmación de correo para pruebas
    options.Password.RequireDigit = true;           // Contraseñas fuertes
    options.Password.RequiredLength = 8;            // Longitud mínima de 8
    options.Password.RequireUppercase = true;       // Al menos una letra mayúscula
})
.AddEntityFrameworkStores<TiendaContext>()
.AddDefaultTokenProviders(); // Proveedores de tokens para autenticación y recuperación de contraseñas

builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7242/"); // URL base de la API de servicios
});

// Agregar controladores con vistas
builder.Services.AddControllersWithViews();

// Registrar servicios de Application e Infrastructure
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

// Agregar AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Configurar servicios adicionales específicos
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Configurar el entorno de desarrollo o producción
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Configuración intermedia
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); // Habilitar autenticación
app.UseAuthorization();  // Habilitar autorización

// Mapear rutas de controladores MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ejecutar la aplicación
app.Run();
