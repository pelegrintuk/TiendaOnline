using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.Application;
using TiendaOnline.Core.Entities;
using TiendaOnline.DAL;
using TiendaOnline.DAL.Data;
using TiendaOnline.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Configuraci�n de cadenas de conexi�n
var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var tiendaConnection = builder.Configuration.GetConnectionString("TiendaConnection")
    ?? throw new InvalidOperationException("Connection string 'TiendaConnection' not found.");

// Configuraci�n del contexto de datos para Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(defaultConnection, b => b.MigrationsAssembly("TiendaOnline.Services")));

// Configuraci�n del contexto de datos para la l�gica de negocio
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlServer(tiendaConnection, b => b.MigrationsAssembly("TiendaOnline.Services")));

// Configuraci�n de Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Cambiar seg�n necesidad
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Registro de AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Configuraci�n de servicios personalizados
builder.Services.AddApplication(); // Esto requiere que tengas un m�todo de extensi�n en Application
builder.Services.AddInfrastructure(); // Esto tambi�n depende de tus m�todos definidos

// Configuraci�n del controlador
builder.Services.AddControllers();

//A�adimos AutoMapper
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
        dbContext.Database.Migrate(); // Aplicar migraciones pendientes autom�ticamente
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Ocurri� un error al aplicar las migraciones");
    }
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Logger.LogInformation("Servicios configurados y corriendo en {Urls}", app.Urls);

app.Run();
