using Microsoft.AspNetCore.Mvc;
using CloudSync.Modules.UserManagement.Controllers;
using CloudSync.Modules.UserManagement.Services.Interfaces;
using Moq;
using Shared.Requests.UserManagement;
using Shared.Responses.UserManagement;

namespace Tests.Unit.UserManagement.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object);
        }

        [Fact]
        public async Task GoogleLogin_WithValidRequest_ReturnsOkWithServiceResponse()
        {
            // Arrange
            var request = new GoogleLoginRequest
            {
                IdToken = "test-token"
            };

            var expectedResponse = new GoogleLoginResponse
            {
                JsonWebToken = "jwt-token-123",
                User = new UserResponse
                {
                    Id = 1,
                    Email = "user@example.com",
                    CreateDateTime = DateTime.UtcNow.AddDays(-10),
                    LastLoginDateTime = DateTime.UtcNow,
                    UserSettings = "{}"
                }
            };

            _authServiceMock
                .Setup(s => s.LoginAsync(request))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.GoogleLogin(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Same(expectedResponse, okResult.Value);

            _authServiceMock.Verify(s => s.LoginAsync(request), Times.Once);
        }

        [Fact]
        public async Task GoogleLogin_PropagatesException_WhenServiceThrows()
        {
            // Arrange
            var request = new GoogleLoginRequest
            {
                IdToken = "test-token"
            };

            _authServiceMock
                .Setup(s => s.LoginAsync(request))
                .ThrowsAsync(new InvalidOperationException("Auth failed"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.GoogleLogin(request));

            _authServiceMock.Verify(s => s.LoginAsync(request), Times.Once);
        }

        [Fact]
        public async Task GoogleLogin_WhenRequestIsNull_PropagatesServiceException()
        {
            // Arrange
            GoogleLoginRequest? request = null;

            _authServiceMock
                .Setup(s => s.LoginAsync(null!))
                .ThrowsAsync(new ArgumentNullException(nameof(request)));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _controller.GoogleLogin(request!));

            _authServiceMock.Verify(s => s.LoginAsync(null!), Times.Once);
        }
    }
}