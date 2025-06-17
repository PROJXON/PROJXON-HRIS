using AutoMapper;
using CloudSync.Modules.UserManagement.Mappings;
using CloudSync.Modules.UserManagement.Repositories.Interfaces;
using CloudSync.Modules.UserManagement.Services;
using Moq;
using CloudSync.Modules.UserManagement.Models;
using Shared.UserManagement.Requests;

namespace Tests.UserManagement.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserService _userService;

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
            new User
            {
                Id = 1,
                GoogleUserId = "string",
                Email = "test@example.com",
                CreateDateTime = DateTime.UtcNow.AddDays(-10),
                LastLoginDateTime = DateTime.UtcNow,
                UserSettings = null,
            }
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
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());

        var result = await _userService.GetAllAsync();

        Assert.Empty(result);
    }
    
    [Fact]
    public async Task GetByIdAsync_ReturnsMappedUserResponse()
    {
        // Arrange
        int userId = 123;
        var user = new User
        {
            Id = userId,
            GoogleUserId = "string",
            Email = "test@example.com",
            CreateDateTime = DateTime.UtcNow.AddDays(-10),
            LastLoginDateTime = DateTime.UtcNow.AddDays(-1),
            UserSettings = null
        };
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.CreateDateTime, result.CreateDateTime);
        Assert.Equal(user.LastLoginDateTime, result.LastLoginDateTime);
        Assert.Equal(user.UserSettings, result.UserSettings);
    }

    [Fact]
    public async Task UpdateAsync_CallsRepositoryUpdate_WhenUserDtoIsNotNull()
    {
        // Arrange
        int userId = 123;
        var updateUserRequest = new UpdateUserRequest
        {
            Email = "updated@example.com",
            GoogleUserId = "blah"
            // other properties if any
        };

        // Act
        await _userService.UpdateAsync(userId, updateUserRequest);

        // Assert
        _userRepositoryMock.Verify(r => r.UpdateAsync(userId, updateUserRequest), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsArgumentNullException_WhenUserDtoIsNull()
    {
        // Arrange
        int userId = 123;
        UpdateUserRequest? updateUserRequest = new UpdateUserRequest
        {
            Email = "updated@example.com",
            GoogleUserId = "blah"
            // other properties if any
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _userService.UpdateAsync(userId, updateUserRequest));
    }

    [Fact]
    public async Task DeleteAsync_CallsRepositoryDelete()
    {
        // Arrange
        int userId = 123;

        // Act
        await _userService.DeleteAsync(userId);

        // Assert
        _userRepositoryMock.Verify(r => r.DeleteAsync(userId), Times.Once);
    }
}