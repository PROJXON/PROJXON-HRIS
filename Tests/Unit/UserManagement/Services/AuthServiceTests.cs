using AutoMapper;
using CloudSync.Exceptions.Business;
using CloudSync.Modules.UserManagement.Models;
using CloudSync.Modules.UserManagement.Repositories.Interfaces;
using CloudSync.Modules.UserManagement.Services;
using CloudSync.Modules.UserManagement.Services.Interfaces;
using Google.Apis.Auth;
using Moq;
using Shared.Enums.UserManagement;
using Shared.Requests.UserManagement;
using Shared.Responses.UserManagement;
using Tests.TestInfrastructure.Builders.UserManagement;
using ValidationException = CloudSync.Exceptions.Business.ValidationException;

namespace Tests.Unit.UserManagement.Services;

public class AuthServiceTests
{
    private readonly Mock<IInvitedUserRepository> _mockInvitedUserRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IGoogleTokenValidator> _mockGoogleTokenValidator;
    private readonly Mock<IJwtTokenService> _mockJwtTokenService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly AuthService _authService;
    
    private readonly GoogleLoginRequest _validRequest;
    private readonly GoogleJsonWebSignature.Payload _validPayload;
    private readonly User _existingUser;
    private readonly InvitedUser _invitedUser;
    private readonly UserResponse _userResponse;
    private const string TestToken = "test-jwt-token";
    private const string TestEmail = "test@example.com";
    private const string TestGoogleUserId = "google-user-123";

    public AuthServiceTests()
    {
        _mockInvitedUserRepository = new Mock<IInvitedUserRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockGoogleTokenValidator = new Mock<IGoogleTokenValidator>();
        _mockJwtTokenService = new Mock<IJwtTokenService>();
        _mockMapper = new Mock<IMapper>();
        
        _authService = new AuthService(
            _mockInvitedUserRepository.Object,
            _mockUserRepository.Object,
            _mockGoogleTokenValidator.Object,
            _mockJwtTokenService.Object,
            _mockMapper.Object
        );
        
        // Setup test data
        _validRequest = new GoogleLoginRequest { IdToken = "valid-google-token" };
        
        _validPayload = new GoogleJsonWebSignature.Payload
        {
            Subject = TestGoogleUserId,
            Email = TestEmail
        };

        _existingUser = new UserTestDataBuilder()
            .WithEmail(TestEmail)
            .WithGoogleUserId(TestGoogleUserId)
            .Build();
        
        _invitedUser = new InvitedUserTestDataBuilder().Build();
        
        _userResponse = new UserResponse
        {
            Id = 1,
            Email = TestEmail
        };
    }

    #region LoginAsync Tests

    [Fact]
    public async Task LoginAsync_WithExistingUser_ReturnsSuccessfulLogin()
    {
        // Arrange
        _mockGoogleTokenValidator
            .Setup(x => x.ValidateAsync(_validRequest))
            .ReturnsAsync(_validPayload);
            
        _mockUserRepository
            .Setup(x => x.GetByGoogleUserIdAsync(TestGoogleUserId))
            .ReturnsAsync(_existingUser);
            
        _mockMapper
            .Setup(x => x.Map<UserResponse>(_existingUser))
            .Returns(_userResponse);
            
        _mockJwtTokenService
            .Setup(x => x.GenerateToken(_existingUser.Email))
            .Returns(TestToken);

        // Act
        var result = await _authService.LoginAsync(_validRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TestToken, result.JsonWebToken);
        Assert.Equal(_userResponse, result.User);
        
        _mockUserRepository.Verify(x => x.UpdateLastLoginTimeAsync(_existingUser.Id), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithInvitedUserPendingStatus_CreatesNewUser()
    {
        // Arrange
        var newUser = new User { Id = 2, Email = TestEmail, GoogleUserId = TestGoogleUserId };
        
        _mockGoogleTokenValidator
            .Setup(x => x.ValidateAsync(_validRequest))
            .ReturnsAsync(_validPayload);
            
        _mockUserRepository
            .Setup(x => x.GetByGoogleUserIdAsync(TestGoogleUserId))
            .ReturnsAsync((User?)null);
            
        _mockInvitedUserRepository
            .Setup(x => x.GetByEmailAsync(TestEmail))
            .ReturnsAsync(_invitedUser);
            
        _mockUserRepository
            .Setup(x => x.CreateUserFromInvitationAsync(_invitedUser, TestGoogleUserId))
            .ReturnsAsync(newUser);
            
        _mockMapper
            .Setup(x => x.Map<UserResponse>(newUser))
            .Returns(_userResponse);
            
        _mockJwtTokenService
            .Setup(x => x.GenerateToken(TestEmail))
            .Returns(TestToken);

        // Act
        var result = await _authService.LoginAsync(_validRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TestToken, result.JsonWebToken);
        Assert.Equal(_userResponse, result.User);
    }

    [Fact]
    public async Task LoginAsync_WithInvitedUserAcceptedStatus_ThrowsDuplicateEntityException()
    {
        // Arrange
        var acceptedInvitedUser = new InvitedUserTestDataBuilder()
            .WithStatus(InvitedUserStatus.Accepted)
            .Build();
        
        _mockGoogleTokenValidator
            .Setup(x => x.ValidateAsync(_validRequest))
            .ReturnsAsync(_validPayload);
            
        _mockUserRepository
            .Setup(x => x.GetByGoogleUserIdAsync(TestGoogleUserId))
            .ReturnsAsync((User?)null);
            
        _mockInvitedUserRepository
            .Setup(x => x.GetByEmailAsync(TestEmail))
            .ReturnsAsync(acceptedInvitedUser);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DuplicateEntityException>(
            () => _authService.LoginAsync(_validRequest));
        Assert.Equal("User has already accepted invitation.", exception.Message);
    }

    [Fact]
    public async Task LoginAsync_WithNonInvitedUser_ThrowsEntityNotFoundException()
    {
        // Arrange
        _mockGoogleTokenValidator
            .Setup(x => x.ValidateAsync(_validRequest))
            .ReturnsAsync(_validPayload);
            
        _mockUserRepository
            .Setup(x => x.GetByGoogleUserIdAsync(TestGoogleUserId))
            .ReturnsAsync((User?)null);
            
        _mockInvitedUserRepository
            .Setup(x => x.GetByEmailAsync(TestEmail))
            .ReturnsAsync((InvitedUser?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _authService.LoginAsync(_validRequest));
        Assert.Equal("User has not been invited.", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task LoginAsync_WithInvalidIdToken_ThrowsValidationException(string invalidToken)
    {
        // Arrange
        var invalidRequest = new GoogleLoginRequest { IdToken = invalidToken };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => _authService.LoginAsync(invalidRequest));
        Assert.Equal("Missing ID token.", exception.Message);
    }

    [Fact]
    public async Task LoginAsync_WhenGoogleTokenValidationFails_PropagatesException()
    {
        // Arrange
        var authException = new AuthenticationException("Invalid Google token");
        _mockGoogleTokenValidator
            .Setup(x => x.ValidateAsync(_validRequest))
            .ThrowsAsync(authException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AuthenticationException>(
            () => _authService.LoginAsync(_validRequest));
        Assert.Equal("Invalid Google token", exception.Message);
    }

    #endregion

    #region Integration-style Tests

    [Fact]
    public async Task LoginAsync_ExistingUserFlow_CallsAllExpectedMethods()
    {
        // Arrange
        _mockGoogleTokenValidator
            .Setup(x => x.ValidateAsync(_validRequest))
            .ReturnsAsync(_validPayload);
            
        _mockUserRepository
            .Setup(x => x.GetByGoogleUserIdAsync(TestGoogleUserId))
            .ReturnsAsync(_existingUser);
            
        _mockMapper
            .Setup(x => x.Map<UserResponse>(_existingUser))
            .Returns(_userResponse);
            
        _mockJwtTokenService
            .Setup(x => x.GenerateToken(_existingUser.Email))
            .Returns(TestToken);

        // Act
        await _authService.LoginAsync(_validRequest);

        // Assert
        _mockGoogleTokenValidator.Verify(x => x.ValidateAsync(_validRequest), Times.Once);
        _mockUserRepository.Verify(x => x.GetByGoogleUserIdAsync(TestGoogleUserId), Times.Once);
        _mockUserRepository.Verify(x => x.UpdateLastLoginTimeAsync(_existingUser.Id), Times.Once);
        _mockMapper.Verify(x => x.Map<UserResponse>(_existingUser), Times.Once);
        _mockJwtTokenService.Verify(x => x.GenerateToken(_existingUser.Email), Times.Once);
        
        // Verify invited user repository is not called for existing users
        _mockInvitedUserRepository.Verify(x => x.GetByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_NewUserFromInvitationFlow_CallsAllExpectedMethods()
    {
        // Arrange
        var newUser = new User { Id = 2, Email = TestEmail, GoogleUserId = TestGoogleUserId };
        
        _mockGoogleTokenValidator
            .Setup(x => x.ValidateAsync(_validRequest))
            .ReturnsAsync(_validPayload);
            
        _mockUserRepository
            .Setup(x => x.GetByGoogleUserIdAsync(TestGoogleUserId))
            .ReturnsAsync((User?)null);
            
        _mockInvitedUserRepository
            .Setup(x => x.GetByEmailAsync(TestEmail))
            .ReturnsAsync(_invitedUser);
            
        _mockUserRepository
            .Setup(x => x.CreateUserFromInvitationAsync(_invitedUser, TestGoogleUserId))
            .ReturnsAsync(newUser);
            
        _mockMapper
            .Setup(x => x.Map<UserResponse>(newUser))
            .Returns(_userResponse);
            
        _mockJwtTokenService
            .Setup(x => x.GenerateToken(TestEmail))
            .Returns(TestToken);

        // Act
        await _authService.LoginAsync(_validRequest);

        // Assert
        _mockGoogleTokenValidator.Verify(x => x.ValidateAsync(_validRequest), Times.Once);
        _mockUserRepository.Verify(x => x.GetByGoogleUserIdAsync(TestGoogleUserId), Times.Once);
        _mockInvitedUserRepository.Verify(x => x.GetByEmailAsync(TestEmail), Times.Once);
        _mockUserRepository.Verify(x => x.CreateUserFromInvitationAsync(_invitedUser, TestGoogleUserId), Times.Once);
        _mockMapper.Verify(x => x.Map<UserResponse>(newUser), Times.Once);
        _mockJwtTokenService.Verify(x => x.GenerateToken(TestEmail), Times.Once);
        
        // Verify last login time is not updated for new users
        _mockUserRepository.Verify(x => x.UpdateLastLoginTimeAsync(It.IsAny<int>()), Times.Never);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task LoginAsync_WhenCreateUserFromInvitationFails_PropagatesException()
    {
        // Arrange
        var expectedMessage = "Database error";
        
        _mockGoogleTokenValidator
            .Setup(x => x.ValidateAsync(_validRequest))
            .ReturnsAsync(_validPayload);
            
        _mockUserRepository
            .Setup(x => x.GetByGoogleUserIdAsync(TestGoogleUserId))
            .ReturnsAsync((User?)null);
            
        _mockInvitedUserRepository
            .Setup(x => x.GetByEmailAsync(TestEmail))
            .ReturnsAsync(_invitedUser);
            
        _mockUserRepository
            .Setup(x => x.CreateUserFromInvitationAsync(_invitedUser, TestGoogleUserId))
            .ThrowsAsync(new AuthenticationException(expectedMessage));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AuthenticationException>(
            () => _authService.LoginAsync(_validRequest));
        Assert.Equal("Database error", exception.Message);
    }

    #endregion
}