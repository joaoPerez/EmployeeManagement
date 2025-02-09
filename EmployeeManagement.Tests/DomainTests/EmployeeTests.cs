using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Tests.DomainTests
{
    public class EmployeeTests
    {
        [Fact]
        public void CreateEmployee_WithValidData_ShouldSucceed()
        {
            var phones = new List<string> { "123456789" };
            var result = Employee.Create(
                "John",
                "Doe",
                "john.doe@example.com",
                "DOC123",
                phones,
                Role.Employee,
                string.Empty,
                new DateTime(1990, 1, 1),
                null);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
        }

        [Fact]
        public void CreateEmployee_Underage_ShouldFail()
        {
            var phones = new List<string> { "123456789" };
            var underageDate = DateTime.Today.AddYears(-17);
            var result = Employee.Create(
                "Jane",
                "Doe",
                "jane.doe@example.com",
                "DOC124",
                phones,
                Role.Employee,
                string.Empty,
                underageDate,
                null);
            Assert.False(result.IsSuccess);
            Assert.Contains("Employee must be at least 18 years old.", result.Errors);
        }

        [Fact]
        public void CreateEmployee_WithoutPhone_ShouldFail()
        {
            var result = Employee.Create(
                "Jane",
                "Doe",
                "jane.doe@example.com",
                "DOC125",
                [],
                Role.Employee,
                string.Empty,
                new DateTime(1990, 1, 1),
                null);
            Assert.False(result.IsSuccess);
            Assert.Contains("At least one phone number is required.", result.Errors);
        }
    }
}