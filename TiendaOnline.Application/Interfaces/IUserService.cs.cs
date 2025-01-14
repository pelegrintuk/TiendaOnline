using System.Collections.Generic;
using System.Threading.Tasks;
using TiendaOnline.Application.DTOs;

namespace TiendaOnline.Application.Interfaces
{
    public interface IUserService
    {
        Task<bool> CreateUserAsync(UserDto userDto);
        Task<UserDto> GetUserByIdAsync(string userId);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task UpdateUserAsync(UserDto userDto);
        Task DeleteUserAsync(string userId);
    }
}
