using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.DAL; // Importar TiendaContext
using TiendaOnline.Core.Entities; // Importar ApplicationUser
using TiendaOnline.Application;

var builder = WebApplication.CreateBuilder(args);

// Configurar la cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("TiendaDB")
    ?? throw new InvalidOperationException("Connection string 'TiendaDB' not found.");

// Configurar el contexto de datos con EF Core
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlServer(connectionString));

// Configurar Identity con la clase personalizada ApplicationUser
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;  // Confirmación de correo
    options.Password.RequireDigit = true;           // Contraseñas fuertes
    options.Password.RequiredLength = 8;            // Longitud mínima de 8
    options.Password.RequireUppercase = true;       // Al menos una letra mayúscula
})
.AddEntityFrameworkStores<TiendaContext>();

// Agregar soporte para Razor Pages
builder.Services.AddRazorPages();

// Registrar servicios de APPLICATION
builder.Services.AddApplication();

// Otros servicios (DB Context, Identity, etc.)
builder.Services.AddControllersWithViews();

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();  // Habilitar autenticación
app.UseAuthorization();   // Habilitar autorización

// Mapear rutas de Razor Pages para Identity
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
