using AutoMapper;
using CloudSync.Modules.UserManagement.Mappings;
using CloudSync.Modules.UserManagement.Repositories.Interfaces;
using CloudSync.Modules.UserManagement.Services;
using Moq;
using CloudSync.Modules.UserManagement.Models;
using Shared.UserManagement.Requests;
using CloudSync.Exceptions.Business;
using Tests.TestInfrastructure.Builders.UserManagement;

namespace Tests.Unit.UserManagement.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserService _userService;
    private const int Id = 123;

    public UserServiceTests()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserMappingProfile>();
        });

        var mapper = mapperConfig.CreateMapper();
        _userRepositoryMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepositoryMock.Object, mapper);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedUserResponses()
    {
        // Arrange
        var users = new List<User>
        {
            new UserTestDataBuilder()
                .WithId(Id)
                .Build()
        };

        _userRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        // Act
        var result = await _userService.GetAllAsync();

        // Assert
        var userResponses = result.ToList();
        Assert.Single(userResponses);

        var response = userResponses[0];
        Assert.Equal(users[0].Id, response.Id);
        Assert.Equal(users[0].Email, response.Email);
        Assert.Equal(users[0].CreateDateTime, response.CreateDateTime);
        Assert.Equal(users[0].LastLoginDateTime, response.LastLoginDateTime);
        Assert.Equal(users[0].UserSettings, response.UserSettings);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoUsersExist()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());

        // Act
        var result = await _userService.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsMappedUserResponse_WhenUserExists()
    {
        // Arrange
        var user = new UserTestDataBuilder()
            .WithId(Id)
            .Build();
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(Id))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetByIdAsync(Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.CreateDateTime, result.CreateDateTime);
        Assert.Equal(user.LastLoginDateTime, result.LastLoginDateTime);
        Assert.Equal(user.UserSettings, result.UserSettings);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsEntityNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(Id))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _userService.GetByIdAsync(Id));
        
        Assert.Equal("User with the given ID does not exist.", exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_CallsRepositoryUpdate_WhenValidRequest()
    {
        // Arrange
        var updateUserRequest = new UpdateUserRequest
        {
            Id = Id,
            Email = "updated@example.com",
            GoogleUserId = "updated-google-id"
        };

        // Act
        await _userService.UpdateAsync(Id, updateUserRequest);

        // Assert
        _userRepositoryMock.Verify(r => r.UpdateAsync(Id, updateUserRequest), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        UpdateUserRequest? updateUserRequest = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _userService.UpdateAsync(Id, updateUserRequest!));
    }

    [Fact]
    public async Task UpdateAsync_ThrowsValidationException_WhenIdMismatch()
    {
        // Arrange
        var updateUserRequest = new UpdateUserRequest
        {
            Id = 456,
            Email = "updated@example.com",
            GoogleUserId = "updated-google-id"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => _userService.UpdateAsync(Id, updateUserRequest));
        
        Assert.Equal("The provided ID does not match the user ID.", exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_CallsRepositoryDelete()
    {
        // Arrange

        // Act
        await _userService.DeleteAsync(Id);

        // Assert
        _userRepositoryMock.Verify(r => r.DeleteAsync(Id), Times.Once);
    }

    // Additional edge case tests
    [Fact]
    public async Task GetAllAsync_VerifiesRepositoryCalledOnce()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());

        // Act
        await _userService.GetAllAsync();

        // Assert
        _userRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_VerifiesRepositoryCalledWithCorrectId()
    {
        // Arrange
        var user = new UserTestDataBuilder()
            .WithId(Id)
            .WithEmail("specific@test.com")
            .CreatedDaysAgo(10)
            .LastLoginDaysAgo(1)
            .Build();
        _userRepositoryMock.Setup(r => r.GetByIdAsync(Id)).ReturnsAsync(user);

        // Act
        await _userService.GetByIdAsync(Id);

        // Assert
        _userRepositoryMock.Verify(r => r.GetByIdAsync(Id), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_VerifiesRepositoryCalledWithCorrectId()
    {
        // Act
        await _userService.DeleteAsync(Id);

        // Assert
        _userRepositoryMock.Verify(r => r.DeleteAsync(Id), Times.Once);
    }
}