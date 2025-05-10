using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using CloudSync.Modules.UserManagement.Models;
using CloudSync.Modules.UserManagement.Controllers;

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

        var controller = new UserController(mockContext.Object);
        var newUser = new User { Id = 1, Name = "Test User" };

        // Act
        var result = await controller.PostUser(newUser);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal("GetUser", createdAtActionResult.ActionName);
        Assert.Equal(newUser.Id, ((User)createdAtActionResult.Value).Id);
        mockSet.Verify(m => m.Add(It.IsAny<User>()), Times.Once);
        mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}