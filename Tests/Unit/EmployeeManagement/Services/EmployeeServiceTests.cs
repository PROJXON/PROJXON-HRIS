using AutoMapper;
using CloudSync.Modules.EmployeeManagement.Mappings;
using CloudSync.Modules.EmployeeManagement.Repositories.Interfaces;
using CloudSync.Modules.EmployeeManagement.Services;
using Moq;
using CloudSync.Modules.EmployeeManagement.Models;
using Shared.EmployeeManagement.Requests;
using Shared.EmployeeManagement.Responses;

namespace Tests.Unit.EmployeeManagement.Services;

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
        var now = DateTime.UtcNow;
        // Arrange
        var employees = new List<Employee>
        {
            new() { Id = 1, BasicInfo = new EmployeeBasic(), ContactInfo = new EmployeeContactInfo(), CreateDateTime = now, Documents = new EmployeeDocuments(), Education = new EmployeeEducation(), Legal = new EmployeeLegal(), PositionDetails = new EmployeePosition(), Training = new EmployeeTraining(), UpdateDateTime = now },
            new() { Id = 2, BasicInfo = new EmployeeBasic(), ContactInfo = new EmployeeContactInfo(), CreateDateTime = now, Documents = new EmployeeDocuments(), Education = new EmployeeEducation(), Legal = new EmployeeLegal(), PositionDetails = new EmployeePosition(), Training = new EmployeeTraining(), UpdateDateTime = now }
        };

        var expectedResponses = new List<EmployeeResponse>
        {
            new() { Id = 1, BasicInfo = new EmployeeBasicResponse(), ContactInfo = new EmployeeContactInfoResponse(), CreateDateTime = now, Documents = new EmployeeDocumentsResponse(), Education = new EmployeeEducationResponse(), Legal = new EmployeeLegalResponse(), PositionDetails = new EmployeePositionResponse(), Training = new EmployeeTrainingResponse(), UpdateDateTime = now },
            new() { Id = 2, BasicInfo = new EmployeeBasicResponse(), ContactInfo = new EmployeeContactInfoResponse(), CreateDateTime = now, Documents = new EmployeeDocumentsResponse(), Education = new EmployeeEducationResponse(), Legal = new EmployeeLegalResponse(), PositionDetails = new EmployeePositionResponse(), Training = new EmployeeTrainingResponse(), UpdateDateTime = now }
        };

        _mockRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(employees);

        // Act
        var result = await _employeeService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(employees.Count, result.Count());
        Assert.Collection(result,
            item => Assert.Equal(1, item.Id),
            item => Assert.Equal(2, item.Id));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnMappedEmployee()
    {
        var now = DateTime.UtcNow;
        // Arrange
        var expectedResponse = new EmployeeResponse { Id = 1, BasicInfo = new EmployeeBasicResponse(), ContactInfo = new EmployeeContactInfoResponse(), CreateDateTime = now, Documents = new EmployeeDocumentsResponse(), Education = new EmployeeEducationResponse(), Legal = new EmployeeLegalResponse(), PositionDetails = new EmployeePositionResponse(), Training = new EmployeeTrainingResponse(), UpdateDateTime = DateTime.UtcNow };

        _mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(new Employee { Id = 1, BasicInfo = new EmployeeBasic(), ContactInfo = new EmployeeContactInfo(), CreateDateTime = now, Documents = new EmployeeDocuments(), Education = new EmployeeEducation(), Legal = new EmployeeLegal(), PositionDetails = new EmployeePosition(), Training = new EmployeeTraining(), UpdateDateTime = DateTime.UtcNow });

        // Act
        var result = await _employeeService.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(expectedResponse.CreateDateTime, result.CreateDateTime);
        Assert.Equal(expectedResponse.BasicInfo.FirstName, result.BasicInfo.FirstName);
    }
    
    [Fact]
    public async Task CreateAsync_ShouldCreateEmployeeAndReturnResponse()
    {
        // Arrange
        var mockRepo = new Mock<IEmployeeRepository>();
        var mockMapper = new Mock<IMapper>();
        var request = new CreateEmployeeRequest
        {
            FirstName = "John",
            LastName = "Doe"
        };
        var employee = new Employee
        {
            BasicInfo = new EmployeeBasic(),
            ContactInfo = new EmployeeContactInfo(),
            Documents = new EmployeeDocuments(),
            Education = new EmployeeEducation(),
            Legal = new EmployeeLegal(),
            PositionDetails = new EmployeePosition(),
            Training = new EmployeeTraining(),
            CreateDateTime = DateTime.UtcNow,
            UpdateDateTime = DateTime.UtcNow
        };
        employee.PositionDetails.Employee = employee;
        employee.Documents.Employee = employee;
        employee.Legal.Employee = employee;
        employee.Education.Employee = employee;
        employee.Training.Employee = employee;
        employee.BasicInfo.FirstName = request.FirstName;
        employee.BasicInfo.LastName = request.LastName;

        var createdEmployee = employee;
        var employeeResponse = new EmployeeResponse{BasicInfo = new EmployeeBasicResponse(), ContactInfo = new EmployeeContactInfoResponse()};

        mockRepo.Setup(r => r.CreateAsync(It.IsAny<Employee>()))
            .ReturnsAsync(createdEmployee);

        mockMapper.Setup(m => m.Map<EmployeeResponse>(createdEmployee))
            .Returns(employeeResponse);

        var service = new EmployeeService(mockRepo.Object, mockMapper.Object);

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(employeeResponse, result);
        mockRepo.Verify(r => r.CreateAsync(It.Is<Employee>(e =>
            e.BasicInfo.FirstName == "John" &&
            e.BasicInfo.LastName == "Doe" &&
            e.PositionDetails!.Employee == e &&
            e.Documents!.Employee == e &&
            e.Legal!.Employee == e &&
            e.Education!.Employee == e &&
            e.Training!.Employee == e
        )), Times.Once);
        mockMapper.Verify(m => m.Map<EmployeeResponse>(createdEmployee), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        var mockRepo = new Mock<IEmployeeRepository>();
        var mockMapper = new Mock<IMapper>();
        var service = new EmployeeService(mockRepo.Object, mockMapper.Object);

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => service.CreateAsync(null!));
    }

    [Fact]
    public async Task CreateAsync_ShouldPropagateRepositoryException()
    {
        // Arrange
        var mockRepo = new Mock<IEmployeeRepository>();
        var mockMapper = new Mock<IMapper>();
        var request = new CreateEmployeeRequest { FirstName = "Jane", LastName = "Smith" };

        mockRepo.Setup(r => r.CreateAsync(It.IsAny<Employee>()))
            .ThrowsAsync(new InvalidOperationException("Repository error"));

        var service = new EmployeeService(mockRepo.Object, mockMapper.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_ShouldPropagateMapperException()
    {
        // Arrange
        var mockRepo = new Mock<IEmployeeRepository>();
        var mockMapper = new Mock<IMapper>();
        var request = new CreateEmployeeRequest { FirstName = "Jane", LastName = "Smith" };
        var employee = new Employee();

        mockRepo.Setup(r => r.CreateAsync(It.IsAny<Employee>()))
            .ReturnsAsync(employee);

        mockMapper.Setup(m => m.Map<EmployeeResponse>(employee))
            .Throws(new Exception("Mapping failed"));

        var service = new EmployeeService(mockRepo.Object, mockMapper.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => service.CreateAsync(request));
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