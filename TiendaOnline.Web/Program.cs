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

// Configurar la cadena de conexi�n desde el archivo de configuraci�n
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Configurar el contexto de datos con EF Core desde TiendaOnline.DAL
builder.Services.AddDAL(builder.Configuration);

// Configurar Identity con la clase personalizada ApplicationUser
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Deshabilitar confirmaci�n de correo para pruebas
    options.Password.RequireDigit = true;           // Contrase�as fuertes
    options.Password.RequiredLength = 8;            // Longitud m�nima de 8
    options.Password.RequireUppercase = true;       // Al menos una letra may�scula
})
.AddEntityFrameworkStores<TiendaContext>()
.AddDefaultTokenProviders(); // Proveedores de tokens para autenticaci�n y recuperaci�n de contrase�as

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

// Configurar servicios adicionales espec�ficos
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Configurar el entorno de desarrollo o producci�n
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Configuraci�n intermedia
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); // Habilitar autenticaci�n
app.UseAuthorization();  // Habilitar autorizaci�n

// Mapear rutas de controladores MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ejecutar la aplicaci�n
app.Run();
