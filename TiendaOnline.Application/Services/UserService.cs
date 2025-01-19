using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
        public async Task<bool> CreateUserAsync(UserDto userDto)
        {
            var user = new ApplicationUser
            {
                UserName = userDto.Name,
                Email = userDto.Email,
                Address = new Address(userDto.Address.Street, userDto.Address.City, userDto.Address.State, userDto.Address.ZipCode, userDto.Address.Country)
            };

            var result = await _userManager.CreateAsync(user, userDto.Password);
            return result.Succeeded;

        }

        public async Task<UserDto> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task UpdateUserAsync(UserDto userDto)
        {
            var user = await _userManager.FindByIdAsync(userDto.Id);
            if (user != null)
            {
                user.UserName = userDto.Name;
                user.Email = userDto.Email;
                user.UpdateAddress(userDto.Address.Street, userDto.Address.City, userDto.Address.State, userDto.Address.ZipCode, userDto.Address.Country);
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
        }
    }
}
