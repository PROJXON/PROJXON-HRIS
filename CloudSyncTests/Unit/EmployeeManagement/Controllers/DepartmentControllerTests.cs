using Microsoft.AspNetCore.Mvc;
using Moq;
using CloudSync.Modules.EmployeeManagement.Controllers;
using CloudSync.Modules.EmployeeManagement.Services.Interfaces;
using CloudSync.Modules.EmployeeManagement.Services.Exceptions;
using Shared.EmployeeManagement.Responses;

namespace Tests.Unit.EmployeeManagement.Controllers
{
    public class DepartmentControllerTests
    {
        private readonly Mock<IDepartmentService> _departmentServiceMock;
        private readonly DepartmentController _controller;

        public DepartmentControllerTests()
        {
            _departmentServiceMock = new Mock<IDepartmentService>();
            _controller = new DepartmentController(_departmentServiceMock.Object);
        }

        [Fact]
        public async Task GetAllDepartments_WhenDepartmentsExist_ReturnsOkWithList()
        {
            // Arrange
            var departments = new List<DepartmentResponse>
            {
                new DepartmentResponse(),
                new DepartmentResponse()
            };

            _departmentServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(departments);

            // Act
            var result = await _controller.GetAllDepartments();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, ok.StatusCode);
            Assert.Same(departments, ok.Value);

            _departmentServiceMock.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllDepartments_WhenNoDepartments_ReturnsOkWithEmptyList()
        {
            // Arrange
            _departmentServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<DepartmentResponse>());

            // Act
            var result = await _controller.GetAllDepartments();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsAssignableFrom<IEnumerable<DepartmentResponse>>(ok.Value);
            Assert.Empty(list);

            _departmentServiceMock.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllDepartments_WhenServiceThrowsDepartmentException_ReturnsStatusCodeWithMessage()
        {
            // Arrange
            var statusCode = 503;
            var message = "Service unavailable";
            var exception = new DepartmentException(message, statusCode);

            _departmentServiceMock.Setup(s => s.GetAllAsync()).ThrowsAsync(exception);

            // Act
            var result = await _controller.GetAllDepartments();

            // Assert
            var obj = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(statusCode, obj.StatusCode);
            Assert.Equal(message, obj.Value);

            _departmentServiceMock.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllDepartments_WhenServiceThrowsUnexpectedException_PropagatesException()
        {
            // Arrange
            _departmentServiceMock.Setup(s => s.GetAllAsync())
                                  .ThrowsAsync(new InvalidOperationException("Unexpected"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.GetAllDepartments());
            _departmentServiceMock.Verify(s => s.GetAllAsync(), Times.Once);
        }
    }
}