using Microsoft.EntityFrameworkCore;
using CloudSync.Modules.UserManagement.Controllers;
using CloudSync.Modules.UserManagement.Models;
using CloudSync.Infrastructure;
using CloudSync.Modules.UserManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.DTOs.User;
using Xunit;

namespace Tests.UserManagement;

public class UserControllerTests
{
    [Fact]
    public async Task PostUser_ShouldAddUserAndReturnCreatedAtAction()
    {
        // Arrange
        var mockSet = new Mock<DbSet<User>>();
        var mockContext = new Mock<DatabaseContext>();
        mockContext.Setup(m => m.Users).Returns(mockSet.Object);
        var passwordAndHash = PasswordService.GeneratePasswordAndHash();

        var controller = new UserController(mockContext.Object);
        var newUser = new User { Id = 1, Username = "Test User", Password = passwordAndHash.HashedPassword};

        // Act
        var result = await controller.CreateUser(newUser);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal("GetUser", (string)createdAtActionResult.ActionName);
        Assert.Equal(newUser.Id, ((CreateUserDTO)createdAtActionResult.Value).Id);
        mockSet.Verify(m => m.Add(It.IsAny<User>()), Times.Once);
        mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}