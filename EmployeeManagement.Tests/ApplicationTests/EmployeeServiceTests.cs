using EmployeeManagement.Application.Services;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Interfaces;
using Moq;

namespace EmployeeManagement.Tests.ApplicationTests
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
        private readonly EmployeeService _employeeService;

        public EmployeeServiceTests()
        {
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _employeeService = new EmployeeService(_employeeRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateEmployee_WithValidData_ShouldSucceed()
        {
            var phones = new List<string> { "123456789" };
            _employeeRepositoryMock.Setup(x => x.GetByDocumentNumberAsync(It.IsAny<string>()))
                .ReturnsAsync((Employee?)null);

            var result = await _employeeService.CreateEmployeeAsync(
                "John",
                "Doe",
                "john@example.com",
                "DOC001",
                phones,
                Role.Employee,
                "password",
                new DateTime(1990, 1, 1),
                null,
                Role.Employee);

            Assert.True(result.IsSuccess);
            _employeeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Employee>()), Times.Once);
        }

        [Fact]
        public async Task CreateEmployee_WithHigherRoleThanCreator_ShouldFail()
        {
            var phones = new List<string> { "123456789" };

            var result = await _employeeService.CreateEmployeeAsync(
                "John",
                "Doe",
                "john@example.com",
                "DOC002",
                phones,
                Role.Leader,
                "password",
                new DateTime(1990, 1, 1),
                null,
                Role.Employee);

            Assert.False(result.IsSuccess);
            Assert.Contains("Insufficient permissions", result.Errors[0]);
        }
    }
}