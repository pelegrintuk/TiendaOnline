using Microsoft.AspNetCore.Mvc;
using TiendaOnline.Application.DTOs;
using TiendaOnline.Application.Interfaces;

namespace TiendaOnline.Services.Controllers
{
    [ApiController]
    [Route("api/Users")]
    public class UsersApiController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersApiController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UserDto userDto)
        {
            if (userId != userDto.Id) return BadRequest();
            await _userService.UpdateUserAsync(userDto);
            return NoContent();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            await _userService.DeleteUserAsync(userId);
            return NoContent();
        }
    }
}
