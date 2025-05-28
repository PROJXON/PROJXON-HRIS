using AutoMapper;
using CloudSync.Modules.UserManagement.Mappings;
using CloudSync.Modules.UserManagement.Models;
using CloudSync.Modules.UserManagement.Repositories.Interfaces;
using CloudSync.Modules.UserManagement.Services;
using CloudSync.Modules.UserManagement.Services.Exceptions;
using CloudSync.Modules.UserManagement.Services.Interfaces;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Moq;
using Shared.Enums.UserManagement;
using Shared.Requests.UserManagement;

namespace Tests.UserManagement.Services;

public class AuthServiceTests
{
    private readonly Mock<IInvitedUserRepository> _invitedUserRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IGoogleTokenValidator> _googleTokenValidatorMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserMappingProfile>();
        });

        var mapper = mapperConfig.CreateMapper();
        
        _invitedUserRepositoryMock = new Mock<IInvitedUserRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _googleTokenValidatorMock = new Mock<IGoogleTokenValidator>();

        // Mock configuration section for JWT settings
        var jwtSectionMock1 = new Mock<IConfigurationSection>();
        jwtSectionMock1.Setup(x => x["Audience"]).Returns("test-audience");
        jwtSectionMock1.Setup(x => x["Issuer"]).Returns("test-issuer");
        jwtSectionMock1.Setup(x => x["Key"]).Returns("super-secret-key-that-is-long-enough");
        jwtSectionMock1.Setup(x => x["ExpiresInMinutes"]).Returns("60");

        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(c => c.GetSection("JWT")).Returns(jwtSectionMock1.Object);

        _authService = new AuthService(configurationMock.Object, _invitedUserRepositoryMock.Object, _userRepositoryMock.Object, _googleTokenValidatorMock.Object, mapper);
    }

    [Fact]
    public async Task LoginAsync_Throws_WhenIdTokenIsMissing()
    {
        var request = new GoogleLoginRequest { IdToken = null! };

        var ex = await Assert.ThrowsAsync<AuthException>(() => _authService.LoginAsync(request));
        Assert.Equal("Missing ID token.", ex.Message);
    }
    

    [Fact]
    public async Task LoginAsync_ReturnsGoogleLoginResponse_WhenExistingUserFound()
    {
        // Arrange
        var googleUserId = "google-subject-123";
        var request = new GoogleLoginRequest { IdToken = "valid-token" };

        var payload = new GoogleJsonWebSignature.Payload
        {
            Subject = googleUserId,
            Email = "user@example.com"
        };

        var existingUser = new User
        {
            Id = 1,
            GoogleUserId = googleUserId,
            Email = payload.Email,
            CreateDateTime = DateTime.UtcNow.AddDays(-10),
            LastLoginDateTime = DateTime.UtcNow.AddDays(-1),
            UserSettings = null,
        };

        _googleTokenValidatorMock.Setup(x => x.ValidateAsync(request)).ReturnsAsync(payload);

        _userRepositoryMock.Setup(r => r.GetByGoogleUserIdAsync(googleUserId))
            .ReturnsAsync(existingUser);

        _userRepositoryMock.Setup(r => r.UpdateLastLoginTimeAsync(existingUser.Id))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(payload.Email, result.User!.Email);
        Assert.False(string.IsNullOrEmpty(result.JsonWebToken));
    }


    [Fact]
    public async Task LoginAsync_Throws_WhenUserNotInvited()
    {
        // Arrange
        var googleUserId = "google-subject-123";
        var request = new GoogleLoginRequest { IdToken = "valid-token" };

        var payload = new GoogleJsonWebSignature.Payload
        {
            Subject = googleUserId,
            Email = "notinvited@example.com"
        };

        _googleTokenValidatorMock.Setup(x => x.ValidateAsync(request)).ReturnsAsync(payload);

        _userRepositoryMock.Setup(r => r.GetByGoogleUserIdAsync(googleUserId))
            .ReturnsAsync((User?)null);

        _invitedUserRepositoryMock.Setup(r => r.GetByEmailAsync(payload.Email))
            .ReturnsAsync((InvitedUser?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AuthException>(() => _authService.LoginAsync(request));
        Assert.Equal("User has not been invited.", ex.Message);
        Assert.Equal(404, ex.StatusCode);
    }

    [Theory]
    [InlineData("Accepted", "User has already accepted invitation.")]
    [InlineData("Expired", "Invitation has expired.")]
    public async Task LoginAsync_Throws_OnInvalidInviteStatus(string status, string expectedMessage)
    {
        // Arrange
        var googleUserId = "google-subject-123";
        var request = new GoogleLoginRequest { IdToken = "valid-token" };

        var payload = new GoogleJsonWebSignature.Payload
        {
            Subject = googleUserId,
            Email = "invited@example.com"
        };

        var invitedUser = new InvitedUser
        {
            Email = payload.Email,
            InvitedByEmployeeId = 1,
            Status = status
        };

        _googleTokenValidatorMock.Setup(x => x.ValidateAsync(request)).ReturnsAsync(payload);

        _userRepositoryMock.Setup(r => r.GetByGoogleUserIdAsync(googleUserId))
            .ReturnsAsync((User?)null);

        _invitedUserRepositoryMock.Setup(r => r.GetByEmailAsync(payload.Email))
            .ReturnsAsync(invitedUser);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AuthException>(() => _authService.LoginAsync(request));
        Assert.Equal(expectedMessage, ex.Message);
    }

    [Fact]
    public async Task LoginAsync_ReturnsResponse_ForPendingInvite()
    {
        // Arrange
        var googleUserId = "google-subject-123";
        var request = new GoogleLoginRequest { IdToken = "valid-token" };

        var payload = new GoogleJsonWebSignature.Payload
        {
            Subject = googleUserId,
            Email = "pending@example.com"
        };

        var invitedUser = new InvitedUser
        {
            Id = 42,
            Email = payload.Email,
            InvitedByEmployeeId = 5,
            Status = nameof(InvitedUserStatus.Pending),
            CreateDateTime = DateTime.UtcNow.AddDays(-5)
        };

        var createdUser = new User
        {
            Id = 99,
            GoogleUserId = googleUserId,
            Email = payload.Email,
            CreateDateTime = DateTime.UtcNow.AddDays(-5),
            LastLoginDateTime = DateTime.UtcNow,
            UserSettings = null,
        };
        
        _userRepositoryMock.Setup(r => r.GetByGoogleUserIdAsync(googleUserId))
            .ReturnsAsync((User?)null);

        _invitedUserRepositoryMock.Setup(r => r.GetByEmailAsync(payload.Email))
            .ReturnsAsync(invitedUser);

        _userRepositoryMock.Setup(r => r.CreateAsync(invitedUser, googleUserId))
            .ReturnsAsync(createdUser);
        
        _googleTokenValidatorMock.Setup(x => x.ValidateAsync(request)).ReturnsAsync(payload);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(payload.Email, result.User!.Email);
        Assert.False(string.IsNullOrEmpty(result.JsonWebToken));
    }
}