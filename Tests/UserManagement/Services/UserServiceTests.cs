using CloudSync.Modules.UserManagement.Models;
using CloudSync.Modules.UserManagement.Repositories.Interfaces;
using CloudSync.Modules.UserManagement.Services;
using Moq;
using Shared.DTOs.UserManagement;

namespace Tests.UserManagement.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> userRepositoryMock;
    private readonly UserService userService;

    public UserServiceTests()
    {
        userRepositoryMock = new Mock<IUserRepository>();
        userService = new UserService(userRepositoryMock.Object);
    }
    [Fact]
    public async Task GetAllAsync_ReturnsMappedUserResponses()
    {
        // Arrange
        var mockRepo = new Mock<IUserRepository>();
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

        mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        var service = new UserService(mockRepo.Object);

        // Act
        var result = await service.GetAllAsync();

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
        var service = new UserService(mockRepo.Object);

        var result = await service.GetAllAsync();

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
        userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await userService.GetByIdAsync(userId);

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
        var userDto = new UserDto
        {
            Email = "updated@example.com",
            // other properties if any
        };

        // Act
        await userService.UpdateAsync(userId, userDto);

        // Assert
        userRepositoryMock.Verify(r => r.UpdateAsync(userId, userDto), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsArgumentNullException_WhenUserDtoIsNull()
    {
        // Arrange
        int userId = 123;
        UserDto? userDto = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => userService.UpdateAsync(userId, userDto!));
    }

    [Fact]
    public async Task DeleteAsync_CallsRepositoryDelete()
    {
        // Arrange
        int userId = 123;

        // Act
        await userService.DeleteAsync(userId);

        // Assert
        userRepositoryMock.Verify(r => r.DeleteAsync(userId), Times.Once);
    }
}