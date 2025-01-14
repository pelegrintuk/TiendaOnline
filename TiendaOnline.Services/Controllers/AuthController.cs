using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TiendaOnline.Core.Entities;
using TiendaOnline.Core.ValueObjects;
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

            var result = await _signInManager.PasswordSignInAsync(loginDto.Username, loginDto.Password, false, false);

            if (!result.Succeeded) return Unauthorized("Credenciales inválidas");

            return Ok("Inicio de sesión exitoso");
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

            return Ok("Usuario registrado exitosamente");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Cierre de sesión exitoso");
        }
    }
}

