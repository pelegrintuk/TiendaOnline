using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using TiendaOnline.Application;
using TiendaOnline.Application.Interfaces;
using TiendaOnline.Application.Mapping;
using TiendaOnline.Application.Services;
using TiendaOnline.Core.Entities;
using TiendaOnline.DAL;
using TiendaOnline.DAL.Data;
using TiendaOnline.Infrastructure.Email;
using TiendaOnline.Infrastructure.Logging; // Asegúrate de importar el espacio de nombres correcto

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
    client.BaseAddress = new Uri("https://localhost:7248/");
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ClientCertificateOptions = ClientCertificateOption.Manual,
    ServerCertificateCustomValidationCallback =
        (httpRequestMessage, cert, certChain, policyErrors) => true
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
builder.Services.AddScoped<ILogService, LogService>(); // Registro de ILogService

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

// Habilitar controladores con vistas y agregar el ModelBinder personalizado
builder.Services.AddControllersWithViews(options =>
{
    options.ModelBinderProviders.Insert(0, new DecimalModelBinderProvider());
});

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

// Establecer la cultura predeterminada a "es-ES"
var cultureInfo = new CultureInfo("es-ES");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// Configurar las culturas soportadas
var supportedCultures = new[] { cultureInfo };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(cultureInfo),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

// Configuración de rutas para controladores MVC
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

// Método SeedRoles
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

// ModelBinder personalizado para decimales
public class DecimalModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType == typeof(decimal) || context.Metadata.ModelType == typeof(decimal?))
            return new DecimalModelBinder();

        return null;
    }
}

public class DecimalModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        // Verificar que bindingContext no sea nulo
        if (bindingContext == null)
            throw new ArgumentNullException(nameof(bindingContext));

        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

        if (valueProviderResult != ValueProviderResult.None && !string.IsNullOrEmpty(valueProviderResult.FirstValue))
        {
            var value = valueProviderResult.FirstValue;

            // Reemplazar separadores decimales según la cultura actual
            value = value.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                         .Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

            if (decimal.TryParse(value, out decimal result))
            {
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            else
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Formato de número no válido.");
            }
        }
        else
        {
            // Valor predeterminado si no hay entrada
            bindingContext.Result = ModelBindingResult.Success(0m);
        }

        return Task.CompletedTask;
    }
}
