using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; // Agregar esta línea
using TiendaOnline.Application.DTOs;
using TiendaOnline.Application.Interfaces;
using TiendaOnline.Core.Entities;

namespace TiendaOnline.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UserService(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.Include(u => u.Address).ToListAsync();
            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

            foreach (var userDto in userDtos)
            {
                var user = users.FirstOrDefault(u => u.Id == userDto.Id);
                var roles = await _userManager.GetRolesAsync(user);
                userDto.Role = roles.FirstOrDefault();
            }

            return userDtos;
        }

        public async Task<UserDto> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return null;
            }

            var userDto = _mapper.Map<UserDto>(user);
            var roles = await _userManager.GetRolesAsync(user);
            userDto.Role = roles.FirstOrDefault();

            return userDto;
        }

        public async Task UpdateUserAsync(UserDto userDto)
        {
            var user = await _userManager.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Id == userDto.Id);

            if (user == null)
            {
                throw new Exception("Usuario no encontrado.");
            }

            // Actualizar manualmente las propiedades necesarias
            user.UserName = userDto.Name;
            user.Email = userDto.Email;

            // Actualizar dirección
            if (userDto.Address != null)
            {
                if (user.Address == null)
                {
                    user.Address = new Address();
                }

                user.Address.Street = userDto.Address.Street;
                user.Address.City = userDto.Address.City;
                user.Address.State = userDto.Address.State;
                user.Address.ZipCode = userDto.Address.ZipCode;
                user.Address.Country = userDto.Address.Country;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Error al actualizar el usuario: {errors}");
            }

            // Actualizar contraseña si se proporcionó
            if (!string.IsNullOrEmpty(userDto.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, userDto.Password);
                if (!passwordResult.Succeeded)
                {
                    var errors = string.Join(", ", passwordResult.Errors.Select(e => e.Description));
                    throw new Exception($"Error al actualizar la contraseña: {errors}");
                }
            }

            // Actualizar roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            var newRole = userDto.Role;

            if (!string.IsNullOrEmpty(newRole))
            {
                var rolesToRemove = currentRoles.Where(r => r != newRole);
                if (rolesToRemove.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                }

                if (!currentRoles.Contains(newRole))
                {
                    await _userManager.AddToRoleAsync(user, newRole);
                }
            }
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    throw new Exception("Error al eliminar el usuario.");
                }
            }
        }

        public async Task<bool> CreateUserAsync(UserDto userDto)
        {
            var user = _mapper.Map<ApplicationUser>(userDto);
            var result = await _userManager.CreateAsync(user, userDto.Password);
            if (!result.Succeeded)
            {
                throw new Exception("Error al crear el usuario.");
            }

            if (!string.IsNullOrEmpty(userDto.Role))
            {
                await _userManager.AddToRoleAsync(user, userDto.Role);
            }

            return result.Succeeded;
        }
    }
}
