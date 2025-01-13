using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TiendaOnline.Services.DTOs;

namespace TiendaOnline.Web.Controllers
    {
        public class AccountController : Controller
        {
            private readonly IHttpClientFactory _httpClientFactory;

            public AccountController(IHttpClientFactory httpClientFactory)
            {
                _httpClientFactory = httpClientFactory;
            }

            public IActionResult Index()
            {
                return View(); // Renderiza Views/Account/Index.cshtml
            }

            public IActionResult Login()
            {
                return View();
            }

            [HttpPost]
            public async Task<IActionResult> Login(LoginDto loginDto)
            {
                if (!ModelState.IsValid) return View(loginDto);

                var client = _httpClientFactory.CreateClient("ApiClient");
                var content = new StringContent(
                    JsonSerializer.Serialize(loginDto),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync("api/Auth/login", content);
                if (response.IsSuccessStatusCode)
                {
                    // Manejar cookies o tokens aquí si es necesario
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Credenciales inválidas");
                return View(loginDto);
            }

            public IActionResult Register()
            {
                return View();
            }

            [HttpPost]
            public async Task<IActionResult> Register(RegisterDto registerDto)
            {
                if (!ModelState.IsValid) return View(registerDto);

                var client = _httpClientFactory.CreateClient("ApiClient");
                var content = new StringContent(
                    JsonSerializer.Serialize(registerDto),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync("api/Auth/register", content);
                if (response.IsSuccessStatusCode)
                {
                    // Redirigir al login tras registro exitoso
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError(string.Empty, "Error al registrar el usuario");
                return View(registerDto);
            }

            [HttpPost]
            public async Task<IActionResult> Logout()
            {
                var client = _httpClientFactory.CreateClient("ApiClient");
                var response = await client.PostAsync("api/Auth/logout", null);

                if (response.IsSuccessStatusCode)
                {
                    // Redirigir a la página principal tras logout exitoso
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Error al cerrar sesión");
                return RedirectToAction("Index", "Home");
            }
        }
    }
