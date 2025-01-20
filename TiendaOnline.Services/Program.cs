using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TiendaOnline.Application;
using TiendaOnline.Application.Interfaces;
using TiendaOnline.Application.Mapping;
using TiendaOnline.Application.Services;
using TiendaOnline.DAL;
using TiendaOnline.DAL.Data;
using TiendaOnline.Core.Entities;
using TiendaOnline.Infrastructure.Email;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Configuración del cliente HTTP
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7248/");
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

// Registro de AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

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

// Configuración de Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configuración de Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TiendaOnline API", Version = "v1" });
});

var app = builder.Build();

// Middleware y enrutamiento
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TiendaOnline API v1"));
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers(); // Asegúrate de mapear los controladores de API

app.Logger.LogInformation("Backend iniciado correctamente en: {Urls}", app.Urls);

app.Run();
