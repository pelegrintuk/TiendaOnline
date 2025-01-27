using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using TiendaOnline.Application.DTOs;
using TiendaOnline.Application.Interfaces;
using TiendaOnline.Application.Services;
using TiendaOnline.Core.Entities;
using Xunit;
using MockQueryable.Moq;
using MockQueryable;

namespace TiendaOnline.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly IUserService _userService;

        public UserServiceTests()
        {
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            _mapperMock = new Mock<IMapper>();
            _userService = new UserService(_userManagerMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<ApplicationUser>
                {
                    new ApplicationUser { Id = "1", UserName = "User1" },
                    new ApplicationUser { Id = "2", UserName = "User2" }
                }.AsQueryable().BuildMockDbSet();
            _userManagerMock.Setup(um => um.Users).Returns(users.Object);
            _mapperMock.Setup(m => m.Map<IEnumerable<UserDto>>(It.IsAny<IEnumerable<ApplicationUser>>())).Returns(new List<UserDto>());

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Assert.NotNull(result);
            _userManagerMock.Verify(um => um.Users, Times.Once);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var user = new ApplicationUser { Id = "1", UserName = "User1" };
            var users = new List<ApplicationUser> { user }.AsQueryable().BuildMockDbSet();
            _userManagerMock.Setup(um => um.Users).Returns(users.Object);
            _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<ApplicationUser>())).Returns(new UserDto());

            // Act
            var result = await _userService.GetUserByIdAsync("1");

            // Assert
            Assert.NotNull(result);
            _userManagerMock.Verify(um => um.Users, Times.Once);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var users = new List<ApplicationUser>().AsQueryable().BuildMockDbSet();
            _userManagerMock.Setup(um => um.Users).Returns(users.Object);

            // Act
            var result = await _userService.GetUserByIdAsync("1");

            // Assert
            Assert.Null(result);
            _userManagerMock.Verify(um => um.Users, Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateUser_WhenUserExists()
        {
            // Arrange
            var userDto = new UserDto { Id = "1", Name = "UpdatedUser", Email = "updated@example.com" };
            var user = new ApplicationUser { Id = "1", UserName = "User1", Email = "user1@example.com" };
            var users = new List<ApplicationUser> { user }.AsQueryable().BuildMockDbSet();
            _userManagerMock.Setup(um => um.Users).Returns(users.Object);
            _userManagerMock.Setup(um => um.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

            // Act
            await _userService.UpdateUserAsync(userDto);

            // Assert
            _userManagerMock.Verify(um => um.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            var userDto = new UserDto { Id = "1", Name = "UpdatedUser", Email = "updated@example.com" };
            var users = new List<ApplicationUser>().AsQueryable().BuildMockDbSet();
            _userManagerMock.Setup(um => um.Users).Returns(users.Object);

            // Act & Assert
            await Assert.ThrowsAsync<System.Exception>(() => _userService.UpdateUserAsync(userDto));
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldDeleteUser_WhenUserExists()
        {
            // Arrange
            var user = new ApplicationUser { Id = "1", UserName = "User1" };
            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.DeleteAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

            // Act
            await _userService.DeleteUserAsync("1");

            // Assert
            _userManagerMock.Verify(um => um.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);

            // Act & Assert
            await Assert.ThrowsAsync<System.Exception>(() => _userService.DeleteUserAsync("1"));
        }
    }
}
