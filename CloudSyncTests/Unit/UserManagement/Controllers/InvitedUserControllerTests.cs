using Microsoft.AspNetCore.Mvc;
using CloudSync.Modules.UserManagement.Controllers;
using CloudSync.Modules.UserManagement.Services.Interfaces;
using Moq;
using Shared.Requests.UserManagement;
using Shared.Responses.UserManagement;

namespace Tests.Unit.UserManagement.Controllers
{
    public class InvitedUserControllerTests
    {
        private readonly Mock<IInvitedUserService> _invitedUserServiceMock;
        private readonly InvitedUserController _controller;

        public InvitedUserControllerTests()
        {
            _invitedUserServiceMock = new Mock<IInvitedUserService>();
            _controller = new InvitedUserController(_invitedUserServiceMock.Object);
        }

        [Fact]
        public async Task GetInvitedUsers_WhenUsersExist_ReturnsOkWithList()
        {
            // Arrange
            var users = new List<InvitedUserResponse>
            {
                new InvitedUserResponse
                {
                    Id = 1,
                    Email = "user1@example.com",
                    InvitedByEmployeeId = 10,
                    Status = default!
                },
                new InvitedUserResponse
                {
                    Id = 2,
                    Email = "user2@example.com",
                    InvitedByEmployeeId = 11,
                    Status = default!
                }
            };

            _invitedUserServiceMock.Setup(s => s.GetAllAsync())
                                   .ReturnsAsync(users);

            // Act
            var result = await _controller.GetInvitedUsers();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, ok.StatusCode);
            Assert.Same(users, ok.Value);

            _invitedUserServiceMock.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetInvitedUsers_WhenNoUsers_ReturnsOkWithEmptyList()
        {
            // Arrange
            _invitedUserServiceMock.Setup(s => s.GetAllAsync())
                                   .ReturnsAsync(new List<InvitedUserResponse>());

            // Act
            var result = await _controller.GetInvitedUsers();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsAssignableFrom<IEnumerable<InvitedUserResponse>>(ok.Value);
            Assert.Empty(list);

            _invitedUserServiceMock.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetInvitedUsers_WhenServiceThrows_PropagatesException()
        {
            // Arrange
            _invitedUserServiceMock.Setup(s => s.GetAllAsync())
                                   .ThrowsAsync(new InvalidOperationException("Service failure"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.GetInvitedUsers());

            _invitedUserServiceMock.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task InviteUser_WithValidRequest_ReturnsOkWithResponse()
        {
            // Arrange
            var request = new InvitedUserRequest
            {
                Email = "new.user@example.com",
                InvitedByEmployeeId = 99
            };

            var expected = new InvitedUserResponse
            {
                Id = 123,
                Email = request.Email,
                InvitedByEmployeeId = request.InvitedByEmployeeId,
                Status = default!
            };

            _invitedUserServiceMock.Setup(s => s.InviteUserAsync(request))
                                   .ReturnsAsync(expected);

            // Act
            var result = await _controller.InviteUser(request);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, ok.StatusCode);
            Assert.Same(expected, ok.Value);

            _invitedUserServiceMock.Verify(s => s.InviteUserAsync(request), Times.Once);
        }

        [Fact]
        public async Task InviteUser_WhenServiceThrows_PropagatesException()
        {
            // Arrange
            var request = new InvitedUserRequest
            {
                Email = "fail.user@example.com",
                InvitedByEmployeeId = 42
            };

            _invitedUserServiceMock.Setup(s => s.InviteUserAsync(request))
                                   .ThrowsAsync(new Exception("Invite failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.InviteUser(request));

            _invitedUserServiceMock.Verify(s => s.InviteUserAsync(request), Times.Once);
        }

        [Fact]
        public async Task InviteUser_WhenRequestIsNull_PropagatesServiceException()
        {
            // Arrange
            InvitedUserRequest? request = null;

            _invitedUserServiceMock.Setup(s => s.InviteUserAsync(null!))
                                   .ThrowsAsync(new ArgumentNullException(nameof(request)));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _controller.InviteUser(request!));

            _invitedUserServiceMock.Verify(s => s.InviteUserAsync(null!), Times.Once);
        }

        [Fact]
        public async Task DeleteInvite_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var id = 123;

            _invitedUserServiceMock.Setup(s => s.DeleteInviteAsync(id))
                                   .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteInvite(id);

            // Assert
            Assert.IsType<NoContentResult>(result);

            _invitedUserServiceMock.Verify(s => s.DeleteInviteAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeleteInvite_WhenServiceThrows_PropagatesException()
        {
            // Arrange
            var id = 456;

            _invitedUserServiceMock.Setup(s => s.DeleteInviteAsync(id))
                                   .ThrowsAsync(new KeyNotFoundException("Invite not found"));

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.DeleteInvite(id));

            _invitedUserServiceMock.Verify(s => s.DeleteInviteAsync(id), Times.Once);
        }
    }
}