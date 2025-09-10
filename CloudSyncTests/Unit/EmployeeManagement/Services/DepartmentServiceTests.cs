using AutoMapper;
using CloudSync.Modules.EmployeeManagement.Models;
using CloudSync.Modules.EmployeeManagement.Repositories.Interfaces;
using CloudSync.Modules.EmployeeManagement.Services;
using Moq;
using Shared.EmployeeManagement.Responses;

namespace Tests.Unit.EmployeeManagement.Services
{
    public class DepartmentServiceTests
    {
        [Fact]
        public async Task GetAllAsync_WhenDepartmentsExist_ReturnsMappedDepartmentList()
        {
            // Arrange
            var mockRepo = new Mock<IDepartmentRepository>();
            var mockMapper = new Mock<IMapper>();
            var service = new DepartmentService(mockRepo.Object, mockMapper.Object);

            var dept1 = new Department();
            var dept2 = new Department();
            var departments = new List<Department> { dept1, dept2 };

            var resp1 = new DepartmentResponse();
            var resp2 = new DepartmentResponse();

            mockRepo.Setup(r => r.GetAllAsync())
                    .ReturnsAsync(departments);

            mockMapper.Setup(m => m.Map<DepartmentResponse>(dept1))
                      .Returns(resp1);
            mockMapper.Setup(m => m.Map<DepartmentResponse>(dept2))
                      .Returns(resp2);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            var list = result.ToList();
            Assert.Equal(2, list.Count);
            Assert.Same(resp1, list[0]);
            Assert.Same(resp2, list[1]);

            mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
            mockMapper.Verify(m => m.Map<DepartmentResponse>(dept1), Times.Once);
            mockMapper.Verify(m => m.Map<DepartmentResponse>(dept2), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_WhenNoDepartments_ReturnsEmptyList()
        {
            // Arrange
            var mockRepo = new Mock<IDepartmentRepository>();
            var mockMapper = new Mock<IMapper>();
            var service = new DepartmentService(mockRepo.Object, mockMapper.Object);

            mockRepo.Setup(r => r.GetAllAsync())
                    .ReturnsAsync(new List<Department>());

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
            mockMapper.Verify(m => m.Map<DepartmentResponse>(It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task GetAllAsync_WhenRepositoryThrows_PropagatesException()
        {
            // Arrange
            var mockRepo = new Mock<IDepartmentRepository>();
            var mockMapper = new Mock<IMapper>();
            var service = new DepartmentService(mockRepo.Object, mockMapper.Object);

            mockRepo.Setup(r => r.GetAllAsync())
                    .ThrowsAsync(new InvalidOperationException("Repo failure"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetAllAsync());

            mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
            mockMapper.Verify(m => m.Map<DepartmentResponse>(It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task GetAllAsync_WhenMapperThrows_PropagatesException()
        {
            // Arrange
            var mockRepo = new Mock<IDepartmentRepository>();
            var mockMapper = new Mock<IMapper>();
            var service = new DepartmentService(mockRepo.Object, mockMapper.Object);

            var dept = new Department();
            mockRepo.Setup(r => r.GetAllAsync())
                    .ReturnsAsync(new List<Department> { dept });

            mockMapper.Setup(m => m.Map<DepartmentResponse>(dept))
                      .Throws(new Exception("Mapping failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.GetAllAsync());

            mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
            mockMapper.Verify(m => m.Map<DepartmentResponse>(dept), Times.Once);
        }
    }
}