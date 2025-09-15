using Microsoft.AspNetCore.Mvc;
using CloudSync.Modules.UserManagement.Controllers;
using CloudSync.Modules.UserManagement.Services.Interfaces;
using Moq;
using Shared.Responses.UserManagement;
using Shared.UserManagement.Requests;

namespace Tests.Unit.UserManagement.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _controller = new UserController(_userServiceMock.Object);
        }

        [Fact]
        public async Task GetUsers_WhenUsersExist_ReturnsOkWithList()
        {
            // Arrange
            var users = new List<UserResponse>
            {
                new UserResponse
                {
                    Id = 1,
                    Email = "user1@example.com",
                    CreateDateTime = DateTime.UtcNow.AddDays(-5),
                    LastLoginDateTime = DateTime.UtcNow.AddDays(-1),
                    UserSettings = "{}"
                },
                new UserResponse
                {
                    Id = 2,
                    Email = "user2@example.com",
                    CreateDateTime = DateTime.UtcNow.AddDays(-7),
                    LastLoginDateTime = DateTime.UtcNow.AddDays(-2),
                    UserSettings = null
                }
            };

            _userServiceMock.Setup(s => s.GetAllAsync())
                            .ReturnsAsync(users);

            // Act
            var result = await _controller.GetUsers();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, ok.StatusCode);
            Assert.Same(users, ok.Value);

            _userServiceMock.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetUsers_WhenNoUsers_ReturnsOkWithEmptyList()
        {
            // Arrange
            _userServiceMock.Setup(s => s.GetAllAsync())
                            .ReturnsAsync(new List<UserResponse>());

            // Act
            var result = await _controller.GetUsers();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsAssignableFrom<IEnumerable<UserResponse>>(ok.Value);
            Assert.Empty(list);

            _userServiceMock.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetUsers_WhenServiceThrows_PropagatesException()
        {
            // Arrange
            _userServiceMock.Setup(s => s.GetAllAsync())
                            .ThrowsAsync(new InvalidOperationException("Service failure"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.GetUsers());

            _userServiceMock.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetUser_WithValidId_ReturnsOkWithUser()
        {
            // Arrange
            var id = 123;
            var user = new UserResponse
            {
                Id = id,
                Email = "user@example.com",
                CreateDateTime = DateTime.UtcNow.AddDays(-10),
                LastLoginDateTime = DateTime.UtcNow.AddDays(-1),
                UserSettings = "{}"
            };

            _userServiceMock.Setup(s => s.GetByIdAsync(id))
                            .ReturnsAsync(user);

            // Act
            var result = await _controller.GetUser(id);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, ok.StatusCode);
            Assert.Same(user, ok.Value);

            _userServiceMock.Verify(s => s.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetUser_WhenServiceThrows_PropagatesException()
        {
            // Arrange
            var id = 999;

            _userServiceMock.Setup(s => s.GetByIdAsync(id))
                            .ThrowsAsync(new KeyNotFoundException("User not found"));

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.GetUser(id));

            _userServiceMock.Verify(s => s.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task PutUser_WithValidRequest_ReturnsNoContent()
        {
            // Arrange
            var id = 5;
            var request = new UpdateUserRequest
            {
                Id = 456,
                Email = "updated@example.com",
                GoogleUserId = "updated-google-id"
            };

            _userServiceMock.Setup(s => s.UpdateAsync(id, request))
                            .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PutUser(id, request);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _userServiceMock.Verify(s => s.UpdateAsync(id, request), Times.Once);
        }

        [Fact]
        public async Task PutUser_WhenServiceThrows_PropagatesException()
        {
            // Arrange
            var id = 42;
            var request = new UpdateUserRequest
            {
                Id = 456,
                Email = "updated@example.com",
                GoogleUserId = "updated-google-id"
            };
            
            _userServiceMock.Setup(s => s.UpdateAsync(id, request))
                            .ThrowsAsync(new InvalidOperationException("Update failed"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.PutUser(id, request));

            _userServiceMock.Verify(s => s.UpdateAsync(id, request), Times.Once);
        }

        [Fact]
        public async Task PutUser_WhenRequestIsNull_PropagatesServiceException()
        {
            // Arrange
            var id = 1;

            _userServiceMock.Setup(s => s.UpdateAsync(id, null!))
                            .ThrowsAsync(new ArgumentNullException("request"));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _controller.PutUser(id, null!));

            _userServiceMock.Verify(s => s.UpdateAsync(id, null!), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var id = 7;

            _userServiceMock.Setup(s => s.DeleteAsync(id))
                            .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteUser(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _userServiceMock.Verify(s => s.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_WhenServiceThrows_PropagatesException()
        {
            // Arrange
            var id = 404;

            _userServiceMock.Setup(s => s.DeleteAsync(id))
                            .ThrowsAsync(new KeyNotFoundException("User not found"));

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.DeleteUser(id));

            _userServiceMock.Verify(s => s.DeleteAsync(id), Times.Once);
        }
    }
}