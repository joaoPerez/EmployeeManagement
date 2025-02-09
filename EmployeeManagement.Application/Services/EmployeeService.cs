using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Interfaces;
using EmployeeManagement.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace EmployeeManagement.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly Microsoft.AspNetCore.Identity.IPasswordHasher<Employee> _passwordHasher;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<Employee>();
        }

        public async Task<Result<Employee>> CreateEmployeeAsync(
            string firstName,
            string lastName,
            string email,
            string documentNumber,
            List<string> phones,
            Role role,
            string password,
            DateTime birthDate,
            Guid? managerId,
            Role creatorRole)
        {
            // Validate permissions: an employee cannot create a leader, and a leader cannot create a director.
            if (!IsAllowedToCreate(creatorRole, role))
                return Result<Employee>.Failure(new List<string> { "Insufficient permissions to create employee with this role." });

            // Check that the document number is unique.
            var existing = await _employeeRepository.GetByDocumentNumberAsync(documentNumber);
            if (existing != null)
                return Result<Employee>.Failure(new List<string> { "Document number already exists." });

            var result = Employee.Create(firstName, lastName, email, documentNumber, phones, role, string.Empty, birthDate, managerId);
            if (!result.IsSuccess)
                return result;

            var employee = result.Value!;

            var hashedPassword = _passwordHasher.HashPassword(employee, password);
            employee.SetPasswordHash(hashedPassword);

            await _employeeRepository.AddAsync(employee);
            return Result<Employee>.Success(employee);
        }

        public async Task<Employee?> GetEmployeeByIdAsync(Guid id) =>
            await _employeeRepository.GetByIdAsync(id);

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync() =>
            await _employeeRepository.GetAllAsync();

        public async Task<Result<Employee>> UpdateEmployeeAsync(
            Guid id,
            string firstName,
            string lastName,
            string email,
            List<string> phones,
            Role role,
            DateTime birthDate,
            Guid? managerId,
            Role updaterRole)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return Result<Employee>.Failure(["Employee not found."]);

            if (!IsAllowedToCreate(updaterRole, role))
                return Result<Employee>.Failure(["Insufficient permissions to update employee with this role."]);

            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(firstName))
            {
                errors.Add("First name is required.");
                firstName = string.Empty;
            }
            if (string.IsNullOrWhiteSpace(lastName))
            {
                errors.Add("Last name is required.");
                lastName = string.Empty;
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add("Email is required.");
                email = string.Empty;
            }
            int age = DateTime.Today.Year - birthDate.Year;
            if (birthDate.Date > DateTime.Today.AddYears(-age))
                age--;
            if (age < 18)
                errors.Add("Employee must be at least 18 years old.");

            if (errors.Any())
                return Result<Employee>.Failure(errors);

            employee.UpdatePersonalInfo(firstName, lastName, email, phones, birthDate, managerId);
            employee.UpdateRole(role);

            await _employeeRepository.UpdateAsync(employee);
            return Result<Employee>.Success(employee);
        }

        public async Task DeleteEmployeeAsync(Guid id) =>
            await _employeeRepository.DeleteAsync(id);

        private bool IsAllowedToCreate(Role creatorRole, Role targetRole)
        {
            if (creatorRole == Role.Director)
                return true;
            if (creatorRole == Role.Leader)
                return targetRole != Role.Director;
            if (creatorRole == Role.Employee)
                return targetRole == Role.Employee;
            return false;
        }
    }
}