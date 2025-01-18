using TiendaOnline.Application.Interfaces;
using TiendaOnline.Application.Mapping;
using TiendaOnline.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuración del cliente HTTP
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7242/");
});

// Registro de AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Registro de servicios
builder.Services.AddScoped<IUserService, UserService>();

// Configuración de cookies de autenticación
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Habilitar controladores con vistas
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Middleware y enrutamiento
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

app.Logger.LogInformation("Frontend iniciado correctamente en: {Urls}", app.Urls);

app.Run();
