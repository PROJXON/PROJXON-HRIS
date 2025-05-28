using AutoMapper;
using CloudSync.Modules.UserManagement.Mappings;
using CloudSync.Modules.UserManagement.Models;
using CloudSync.Modules.UserManagement.Repositories.Interfaces;
using CloudSync.Modules.UserManagement.Services;
using CloudSync.Modules.UserManagement.Services.Exceptions;
using Moq;
using Shared.Enums.UserManagement;
using Shared.Requests.UserManagement;
using Shared.Responses.UserManagement;

namespace Tests.UserManagement.Services;

public class InvitedUserServiceTests
{
    private readonly Mock<IInvitedUserRepository> _repositoryMock;
    private readonly InvitedUserService _service;

    public InvitedUserServiceTests()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<InvitedUserMappingProfile>();
        });

        var mapper = mapperConfig.CreateMapper();
        
        _repositoryMock = new Mock<IInvitedUserRepository>();
        _service = new InvitedUserService(_repositoryMock.Object, mapper);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedInvitedUserResponses()
    {
        // Arrange
        var invitedUsers = new List<InvitedUser>
        {
            new InvitedUser 
            {
                Id = 1,
                Email = "test1@example.com",
                InvitedByEmployeeId = 10,
                Status = "Pending",
                CreateDateTime = DateTime.UtcNow.AddDays(-1)
            },
            new InvitedUser
            {
                Id = 2,
                Email = "test2@example.com",
                InvitedByEmployeeId = 20,
                Status = "Accepted",
                CreateDateTime = DateTime.UtcNow.AddDays(-2)
            }
        };

        _repositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(invitedUsers);

        // Act
        var results = (await _service.GetAllAsync()).ToList();

        // Assert
        Assert.Equal(invitedUsers.Count, results.Count);
        for (int i = 0; i < results.Count; i++)
        {
            Assert.Equal(invitedUsers[i].Id, results[i].Id);
            Assert.Equal(invitedUsers[i].Email, results[i].Email);
            Assert.Equal(invitedUsers[i].InvitedByEmployeeId, results[i].InvitedByEmployeeId);
            Assert.Equal(Enum.Parse<InvitedUserStatus>(invitedUsers[i].Status), results[i].Status);
            Assert.Equal(invitedUsers[i].CreateDateTime, results[i].CreateDateTime);
        }
    }

    [Fact]
    public async Task InviteUserAsync_Throws_WhenEmailIsNullOrWhitespace()
    {
        // Arrange
        var request = new InvitedUserRequest { InvitedByEmployeeId = 1, Email = " " };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvitedUserException>(() => _service.InviteUserAsync(request));
        Assert.Equal("Email is required.", ex.Message);
        Assert.Equal(400, ex.StatusCode);
    }

    [Fact]
    public async Task InviteUserAsync_Throws_WhenEmailAlreadyInvited()
    {
        // Arrange
        var request = new InvitedUserRequest { Email = "already@invited.com", InvitedByEmployeeId = 5 };
        _repositoryMock.Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync(new InvitedUser
            {
                Id = 1,
                Email = "null",
                InvitedByEmployeeId = 0,
                InvitedByEmployee = null,
                CreateDateTime = default,
                Status = nameof(InvitedUserStatus.Accepted)
            }); // simulate existing invite

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvitedUserException>(() => _service.InviteUserAsync(request));
        Assert.Equal("Email has already been invited.", ex.Message);
        Assert.Equal(409, ex.StatusCode);
    }

    [Fact]
    public async Task InviteUserAsync_AddsNewInviteAndReturnsResponse()
    {
        // Arrange
        var request = new InvitedUserRequest { Email = "new@invite.com", InvitedByEmployeeId = 7 };
        _repositoryMock.Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync((InvitedUser?)null); // no existing invite

        var addedDto = new InvitedUser
        {
            Id = 99,
            Email = request.Email,
            InvitedByEmployeeId = request.InvitedByEmployeeId,
            Status = nameof(InvitedUserStatus.Pending),
            CreateDateTime = DateTime.UtcNow
        };

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<InvitedUserRequest>()))
            .ReturnsAsync(addedDto);

        // Act
        var result = await _service.InviteUserAsync(request);

        // Assert
        var okResult = Assert.IsType<InvitedUserResponse>(result.Value);
        Assert.Equal(addedDto.Id, okResult.Id);
        Assert.Equal(addedDto.Email, okResult.Email);
        Assert.Equal(addedDto.InvitedByEmployeeId, okResult.InvitedByEmployeeId);
        Assert.Equal(Enum.Parse<InvitedUserStatus>(addedDto.Status), okResult.Status);
        Assert.Equal(addedDto.CreateDateTime, okResult.CreateDateTime);
    }

    [Fact]
    public async Task DeleteInviteAsync_CallsRepositoryDelete()
    {
        // Arrange
        int idToDelete = 42;

        // Act
        await _service.DeleteInviteAsync(idToDelete);

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync(idToDelete), Times.Once);
    }
}
