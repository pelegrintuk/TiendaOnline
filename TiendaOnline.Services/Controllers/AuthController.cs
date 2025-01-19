using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TiendaOnline.Core.Entities;
using TiendaOnline.Services.DTOs;
using TiendaOnline.Application.DTOs;

namespace TiendaOnline.Services.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _signInManager.PasswordSignInAsync(loginDto.Username, loginDto.Password, isPersistent: false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return Unauthorized(new { message = "Credenciales inválidas" });
            }

            return Ok(new { message = "Inicio de sesión exitoso" });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new ApplicationUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email
            };

            user.UpdateAddress(
                registerDto.Street,
                registerDto.City,
                registerDto.State,
                registerDto.ZipCode,
                registerDto.Country
            );

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Usuario registrado exitosamente" });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Cierre de sesión exitoso" });
        }

        [HttpGet("currentuser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email,
                // Mapear otros campos necesarios
            };

            return Ok(userDto);
        }
    }
}

