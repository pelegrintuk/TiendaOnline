using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TiendaOnline.Application;
using TiendaOnline.Application.Interfaces;
using TiendaOnline.Application.Mapping;
using TiendaOnline.Application.Services;
using TiendaOnline.DAL;
using TiendaOnline.DAL.Data;
using TiendaOnline.Core.Entities;
using TiendaOnline.Infrastructure.Email;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

try
{
    // Configuración de logging
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    builder.Logging.AddDebug();

    // Configuración del cliente HTTP
    builder.Services.AddHttpClient("ApiClient", client =>
    {
        client.BaseAddress = new Uri("https://localhost:7248/"); // URL base de la API de servicios
    });

    // Registro de AutoMapper
    builder.Services.AddAutoMapper(typeof(MappingProfile));

    // Configuración de Identity
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    // Registro de servicios
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IOrderService, OrderService>();
    builder.Services.AddScoped<ICartService, CartService>();
    builder.Services.AddScoped<IEmailService, EmailService>(); // Registro de IEmailService

    // Configuración de cookies de autenticación
    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

    // Habilitar controladores con vistas
    builder.Services.AddControllersWithViews();

    // Configuración del contexto de datos
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Registrar servicios de la capa Application y DAL
    builder.Services.AddApplication();
    builder.Services.AddDAL(builder.Configuration);

    var app = builder.Build();

    var logger = app.Services.GetRequiredService<ILogger<Program>>();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.MapControllers(); // Asegúrate de mapear los controladores de API

    logger.LogInformation("Frontend iniciado correctamente en: {Urls}", app.Urls);

    app.Run();
}
catch (Exception ex)
{
    var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during application startup");
    throw;
}
