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

// Configuración de logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

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
    client.BaseAddress = new Uri("https://localhost:7248/"); // URL base de la API de servicios
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
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
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers(); // Asegúrate de mapear los controladores de API

// Crear roles automáticamente
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await SeedRoles(roleManager);
}

logger.LogInformation("Frontend iniciado correctamente en: {Urls}", app.Urls);

app.Run();

async Task SeedRoles(RoleManager<IdentityRole> roleManager)
{
    var roles = new[] { "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

/*
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

    // Asegúrate de mapear los controladores de API
    // Comentar la parte de SeedData
    
    // Agregar productos automáticamente
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApplicationDbContext>();
        await SeedData(context);
    }
    

// Crear roles automáticamente
using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await SeedRoles(roleManager);
    }

    logger.LogInformation("Frontend iniciado correctamente en: {Urls}", app.Urls);

    app.Run();
}
catch (Exception ex)
{
    var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during application startup");
    throw;
}

async Task SeedRoles(RoleManager<IdentityRole> roleManager)
{
    var roles = new[] { "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}



// Método SeedData comentado
/*
async Task SeedData(ApplicationDbContext context)
{
    if (!context.Products.Any())
    {
        var products = new List<Product>
        {
            new Product
            {
                Name = "Procesador Intel Core i9",
                Description = "Procesador Intel Core i9 de 10ª generación",
                Price = 499.99m,
                Stock = 50,
                Category = "Procesadores",
                IsFeatured = true,
                OrderProducts = new List<OrderProduct>(),
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "https://m.media-amazon.com/images/I/91YH5DjWbeL._AC_SL1500_.jpg" },
                    new ProductImage { ImageUrl = "https://m.media-amazon.com/images/I/61PX7Uh0LqL._AC_SL1000_.jpg" }
                }
            },
            new Product
            {
                Name = "Tarjeta Gráfica NVIDIA RTX 4060",
                Description = "Tarjeta gráfica NVIDIA GeForce RTX 4060",
                Price = 699.99m,
                Stock = 30,
                Category = "Tarjetas Gráficas",
                IsFeatured = true,
                OrderProducts = new List<OrderProduct>(),
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "https://m.media-amazon.com/images/I/61q0rsE3ezL._AC_SL1500_.jpg" },
                    new ProductImage { ImageUrl = "https://m.media-amazon.com/images/I/81BA5-X8a7L._AC_SL1500_.jpg" }
                }
            },
            new Product
            {
                Name = "Memoria RAM Corsair 16GB",
                Description = "Memoria RAM Corsair Vengeance LPX 16GB (2 x 8GB) DDR4",
                Price = 89.99m,
                Stock = 100,
                Category = "Memoria RAM",
                IsFeatured = false,
                OrderProducts = new List<OrderProduct>(),
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "https://m.media-amazon.com/images/I/51OAieKsYEL._AC_SL1100_.jpg" },
                    new ProductImage { ImageUrl = "https://m.media-amazon.com/images/I/71JJvu1UPML._AC_SL1500_.jpg" }
                }
            },
            new Product
            {
                Name = "Disco Duro SSD Samsung 1TB",
                Description = "Disco duro SSD Samsung 970 EVO Plus 1TB",
                Price = 149.99m,
                Stock = 75,
                Category = "Almacenamiento",
                IsFeatured = true,
                OrderProducts = new List<OrderProduct>(),
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "https://m.media-amazon.com/images/I/61OOAXlbRtL._AC_SL1000_.jpg" },
                    new ProductImage { ImageUrl = "https://m.media-amazon.com/images/I/61dpUfsQ-eL._AC_SL1000_.jpg" }
                }
            },
            new Product
            {
                Name = "Placa Base ASUS ROG",
                Description = "Placa base ASUS ROG Strix Z490-E Gaming",
                Price = 299.99m,
                Stock = 40,
                Category = "Placas Base",
                IsFeatured = false,
                OrderProducts = new List<OrderProduct>(),
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "https://m.media-amazon.com/images/I/61HCjQUuxTL._AC_SL1000_.jpg" },
                    new ProductImage { ImageUrl = "https://m.media-amazon.com/images/I/713KDQeQkHL._AC_SL1138_.jpg" }
                }
            }
        };

        context.Products.AddRange(products);
        await context.SaveChangesAsync();
    }
}
*/