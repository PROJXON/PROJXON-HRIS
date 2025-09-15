using Microsoft.AspNetCore.Mvc;
using Moq;
using CloudSync.Modules.EmployeeManagement.Controllers;
using CloudSync.Modules.EmployeeManagement.Services.Interfaces;
using Shared.EmployeeManagement.Requests;
using Shared.EmployeeManagement.Responses;

namespace Tests.Unit.EmployeeManagement.Controllers
{
    public class EmployeeControllerTests
    {
        private readonly Mock<IEmployeeService> _employeeServiceMock;
        private readonly EmployeeController _controller;

        public EmployeeControllerTests()
        {
            _employeeServiceMock = new Mock<IEmployeeService>();
            _controller = new EmployeeController(_employeeServiceMock.Object);
        }

        private static EmployeeResponse MakeEmployeeResponse(int id) => new EmployeeResponse
        {
            Id = id,
            BasicInfo = new EmployeeBasicResponse(),
            ContactInfo = new EmployeeContactInfoResponse()
        };

        [Fact]
        public async Task GetAllEmployees_WhenEmployeesExist_ReturnsOkWithList()
        {
            // Arrange
            var employees = new List<EmployeeResponse>
            {
                MakeEmployeeResponse(1),
                MakeEmployeeResponse(2)
            };

            _employeeServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(employees);

            // Act
            var result = await _controller.GetAllEmployees();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, ok.StatusCode);
            Assert.Same(employees, ok.Value);

            _employeeServiceMock.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllEmployees_WhenNoEmployees_ReturnsOkWithEmptyList()
        {
            // Arrange
            _employeeServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<EmployeeResponse>());

            // Act
            var result = await _controller.GetAllEmployees();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsAssignableFrom<IEnumerable<EmployeeResponse>>(ok.Value);
            Assert.Empty(list);

            _employeeServiceMock.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllEmployees_WhenServiceThrows_PropagatesException()
        {
            // Arrange
            _employeeServiceMock.Setup(s => s.GetAllAsync())
                                .ThrowsAsync(new InvalidOperationException("Service failure"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.GetAllEmployees());
            _employeeServiceMock.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetById_WithValidId_ReturnsOkWithEmployee()
        {
            // Arrange
            var id = 123;
            var employee = MakeEmployeeResponse(id);

            _employeeServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(employee);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, ok.StatusCode);
            Assert.Same(employee, ok.Value);

            _employeeServiceMock.Verify(s => s.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenServiceThrows_PropagatesException()
        {
            // Arrange
            var id = 999;
            _employeeServiceMock.Setup(s => s.GetByIdAsync(id))
                                .ThrowsAsync(new KeyNotFoundException("Employee not found"));

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.GetById(id));
            _employeeServiceMock.Verify(s => s.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetByDepartment_WithValidId_ReturnsOkWithList()
        {
            // Arrange
            var departmentId = 42;
            var employees = new List<EmployeeResponse>
            {
                MakeEmployeeResponse(1),
                MakeEmployeeResponse(2)
            };

            _employeeServiceMock.Setup(s => s.GetByDepartmentAsync(departmentId))
                                .ReturnsAsync(employees);

            // Act
            var result = await _controller.GetByDepartment(departmentId);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, ok.StatusCode);
            Assert.Same(employees, ok.Value);

            _employeeServiceMock.Verify(s => s.GetByDepartmentAsync(departmentId), Times.Once);
        }

        [Fact]
        public async Task GetByDepartment_WhenNoEmployees_ReturnsOkWithEmptyList()
        {
            // Arrange
            var departmentId = 43;
            _employeeServiceMock.Setup(s => s.GetByDepartmentAsync(departmentId))
                                .ReturnsAsync(new List<EmployeeResponse>());

            // Act
            var result = await _controller.GetByDepartment(departmentId);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsAssignableFrom<IEnumerable<EmployeeResponse>>(ok.Value);
            Assert.Empty(list);

            _employeeServiceMock.Verify(s => s.GetByDepartmentAsync(departmentId), Times.Once);
        }

        [Fact]
        public async Task GetByDepartment_WhenServiceThrows_PropagatesException()
        {
            // Arrange
            var departmentId = 44;
            _employeeServiceMock.Setup(s => s.GetByDepartmentAsync(departmentId))
                                .ThrowsAsync(new InvalidOperationException("Service failure"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.GetByDepartment(departmentId));
            _employeeServiceMock.Verify(s => s.GetByDepartmentAsync(departmentId), Times.Once);
        }

        [Fact]
        public async Task CreateEmployee_WithValidRequest_ReturnsOkWithResponse()
        {
            // Arrange
            var request = new CreateEmployeeRequest { FirstName = "John", LastName = "Doe" };
            var expected = MakeEmployeeResponse(100);

            _employeeServiceMock.Setup(s => s.CreateAsync(request)).ReturnsAsync(expected);

            // Act
            var result = await _controller.CreateEmployee(request);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, ok.StatusCode);
            Assert.Same(expected, ok.Value);

            _employeeServiceMock.Verify(s => s.CreateAsync(request), Times.Once);
        }

        [Fact]
        public async Task CreateEmployee_WhenServiceThrows_PropagatesException()
        {
            // Arrange
            var request = new CreateEmployeeRequest { FirstName = "Jane", LastName = "Smith" };
            _employeeServiceMock.Setup(s => s.CreateAsync(request))
                                .ThrowsAsync(new InvalidOperationException("Create failed"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.CreateEmployee(request));
            _employeeServiceMock.Verify(s => s.CreateAsync(request), Times.Once);
        }

        [Fact]
        public async Task CreateEmployee_WhenRequestIsNull_PropagatesServiceException()
        {
            // Arrange
            CreateEmployeeRequest? request = null;
            _employeeServiceMock.Setup(s => s.CreateAsync(null!))
                                .ThrowsAsync(new ArgumentNullException("request"));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _controller.CreateEmployee(request!));
            _employeeServiceMock.Verify(s => s.CreateAsync(null!), Times.Once);
        }

        [Fact]
        public async Task PutEmployee_WithValidRequest_ReturnsNoContent()
        {
            // Arrange
            var id = 5;
            var request = new UpdateEmployeeRequest {};

            _employeeServiceMock.Setup(s => s.UpdateAsync(id, request)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PutEmployee(id, request);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _employeeServiceMock.Verify(s => s.UpdateAsync(id, request), Times.Once);
        }

        [Fact]
        public async Task PutEmployee_WhenServiceThrows_PropagatesException()
        {
            // Arrange
            var id = 6;
            var request = new UpdateEmployeeRequest {};

            _employeeServiceMock.Setup(s => s.UpdateAsync(id, request))
                                .ThrowsAsync(new InvalidOperationException("Update failed"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.PutEmployee(id, request));
            _employeeServiceMock.Verify(s => s.UpdateAsync(id, request), Times.Once);
        }

        [Fact]
        public async Task PutEmployee_WhenRequestIsNull_PropagatesServiceException()
        {
            // Arrange
            var id = 7;
            _employeeServiceMock.Setup(s => s.UpdateAsync(id, null!))
                                .ThrowsAsync(new ArgumentNullException("request"));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _controller.PutEmployee(id, null!));
            _employeeServiceMock.Verify(s => s.UpdateAsync(id, null!), Times.Once);
        }

        [Fact]
        public async Task DeleteEmployee_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var id = 8;
            _employeeServiceMock.Setup(s => s.DeleteAsync(id)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteEmployee(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _employeeServiceMock.Verify(s => s.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeleteEmployee_WhenServiceThrows_PropagatesException()
        {
            // Arrange
            var id = 9;
            _employeeServiceMock.Setup(s => s.DeleteAsync(id))
                                .ThrowsAsync(new KeyNotFoundException("Employee not found"));

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.DeleteEmployee(id));
            _employeeServiceMock.Verify(s => s.DeleteAsync(id), Times.Once);
        }
    }
}