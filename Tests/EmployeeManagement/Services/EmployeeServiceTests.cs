using AutoMapper;
using CloudSync.Modules.EmployeeManagement.Mappings;
using CloudSync.Modules.EmployeeManagement.Models;
using CloudSync.Modules.EmployeeManagement.Repositories.Interfaces;
using CloudSync.Modules.EmployeeManagement.Services;
using Moq;
using Shared.EmployeeManagement.Requests;
using Shared.EmployeeManagement.Responses;

namespace Tests.EmployeeManagement.Services;

public class EmployeeServiceTests
{
    private readonly Mock<IEmployeeRepository> _mockRepository;
    private readonly EmployeeService _employeeService;

    public EmployeeServiceTests()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<EmployeeMappingProfile>();
        });

        var mapper = mapperConfig.CreateMapper();
        
        _mockRepository = new Mock<IEmployeeRepository>();
        _employeeService = new EmployeeService(_mockRepository.Object, mapper);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedEmployees()
    {
        // Arrange
        var employees = new List<Employee>
        {
        };

        var expectedResponses = new List<EmployeeResponse>
        {
            new() { Id = 1, FirstName = "John", LastName = "Doe" },
            new() { Id = 2, FirstName = "Jane", LastName = "Smith" }
        };

        _mockRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(employees);

        // Act
        var result = await _employeeService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Collection(result,
            item => Assert.Equal(1, item.Id),
            item => Assert.Equal(2, item.Id));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnMappedEmployee()
    {
        // Arrange
        var expectedResponse = new EmployeeResponse { Id = 1, FirstName = "John", LastName = "Doe" };

        _mockRepository.Setup(repo => repo.GetByIdAsync(1));

        // Act
        var result = await _employeeService.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
    }

    [Fact]
    public async Task GetByDepartmentAsync_ShouldThrowNotImplementedException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(() => 
            _employeeService.GetByDepartmentAsync("IT"));
    }

    [Fact]
    public async Task GetByRoleAsync_ShouldThrowNotImplementedException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(() => 
            _employeeService.GetByRoleAsync("Developer"));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowNotImplementedException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(() => 
            _employeeService.CreateAsync(new CreateEmployeeRequest
            {
                FirstName = "Blah",
                LastName = "Bleh"
            }));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotImplementedException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(() => 
            _employeeService.UpdateAsync(1, new UpdateEmployeeRequest()));
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowNotImplementedException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(() => 
            _employeeService.DeleteAsync(1));
    }
}