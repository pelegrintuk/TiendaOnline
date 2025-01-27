using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using TiendaOnline.Application.DTOs;
using TiendaOnline.Application.Interfaces;
using TiendaOnline.Core.Entities;
using TiendaOnline.Services.DTOs;

namespace TiendaOnline.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderService _orderService;
        private readonly ILogService _logService;

        public AccountController(IHttpClientFactory httpClientFactory, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IOrderService orderService, ILogService logService)
        {
            _httpClientFactory = httpClientFactory;
            _signInManager = signInManager;
            _userManager = userManager;
            _orderService = orderService;
            _logService = logService;
        }

        public async Task<IActionResult> Profile()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            var client = _httpClientFactory.CreateClient("ApiClient");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await client.GetFromJsonAsync<UserDto>($"api/Users/{userId}");

            if (user == null)
            {
                _logService.LogError("User not found", new Exception("User not found"));
                return NotFound();
            }

            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            var userProfile = new UserProfileDto
            {
                Name = user.Name,
                Email = user.Email,
                Address = user.Address,
                Orders = orders
            };

            return View(userProfile);
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
                var result = await _signInManager.PasswordSignInAsync(loginDto.Username, loginDto.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Inicio de sesión exitoso.";
                    return RedirectToAction("Index", "Home");
                }
            }

            // Leer el mensaje de error de la API
            var errorResponse = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = !string.IsNullOrEmpty(errorResponse) ? errorResponse : "Error al iniciar sesión.";
            _logService.LogError("Error al iniciar sesión", new Exception(errorResponse));
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
                var user = new ApplicationUser
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email,
                    Address = new Address(registerDto.Street, registerDto.City, registerDto.State, registerDto.ZipCode, registerDto.Country)
                };

                var result = await _userManager.CreateAsync(user, registerDto.Password);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Registro exitoso. Ahora puedes iniciar sesión.";
                    return RedirectToAction("Login");
                }
                else
                {
                    var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                    TempData["ErrorMessage"] = errorMessage;
                    _logService.LogError("Error al registrar el usuario", new Exception(errorMessage));
                }
            }
            else
            {
                var errorMessage = "Error al registrar el usuario.";
                TempData["ErrorMessage"] = errorMessage;
                _logService.LogError(errorMessage, new Exception(errorMessage));
            }

            return View(registerDto);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.PostAsync("api/Auth/logout", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Cierre de sesión exitoso.";
            }
            else
            {
                var errorMessage = "Error al cerrar sesión.";
                TempData["ErrorMessage"] = errorMessage;
                _logService.LogError(errorMessage, new Exception(errorMessage));
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
