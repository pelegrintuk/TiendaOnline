using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.Application;
using TiendaOnline.Core.Entities;
using TiendaOnline.DAL;
using TiendaOnline.DAL.Data;
using TiendaOnline.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Configuración de cadenas de conexión
var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var tiendaConnection = builder.Configuration.GetConnectionString("TiendaConnection")
    ?? throw new InvalidOperationException("Connection string 'TiendaConnection' not found.");

// Configuración del contexto de datos para Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(defaultConnection, b => b.MigrationsAssembly("TiendaOnline.Services")));

// Configuración del contexto de datos para la lógica de negocio
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlServer(tiendaConnection, b => b.MigrationsAssembly("TiendaOnline.Services")));

// Configuración de Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Cambiar según necesidad
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Registro de AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Configuración de servicios personalizados
builder.Services.AddApplication(); // Esto requiere que tengas un método de extensión en Application
builder.Services.AddInfrastructure(); // Esto también depende de tus métodos definidos

// Configuración del controlador
builder.Services.AddControllers();

//Añadimos AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Inicializar migraciones en tiempo de inicio (opcional)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate(); // Aplicar migraciones pendientes automáticamente
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Ocurrió un error al aplicar las migraciones");
    }
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Logger.LogInformation("Servicios configurados y corriendo en {Urls}", app.Urls);

app.Run();
