using CloudSync.Modules.UserManagement.Models;
using CloudSync.Modules.UserManagement.Repositories.Interfaces;
using CloudSync.Modules.UserManagement.Services;
using Moq;

namespace Tests.UserManagement.Services;

public class UserServiceTests
{
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
}