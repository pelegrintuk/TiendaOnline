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
                TempData["SuccessMessage"] = "Inicio de sesión exitoso.";
                return RedirectToAction("Index", "Home");
            }

            // Leer el mensaje de error de la API
            var errorResponse = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = !string.IsNullOrEmpty(errorResponse) ? errorResponse : "Error al iniciar sesión.";
            return View(loginDto);
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Por favor, revisa los datos del formulario.";
                return View(registerDto);
            }

            var client = _httpClientFactory.CreateClient("ApiClient");
            var content = new StringContent(JsonSerializer.Serialize(registerDto), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/Auth/register", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Registro exitoso. Ahora puedes iniciar sesión.";
                return RedirectToAction("Login");
            }

            TempData["ErrorMessage"] = "Error al registrar el usuario.";
            return View(registerDto);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.PostAsync("api/Auth/logout", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Cierre de sesión exitoso.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error al cerrar sesión.";
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
