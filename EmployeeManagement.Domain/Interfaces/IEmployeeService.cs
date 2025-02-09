using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Domain.Interfaces
{
    public interface IEmployeeService
    {
        Task<Result<Employee>> CreateEmployeeAsync(
            string firstName,
            string lastName,
            string email,
            string documentNumber,
            List<string> phones,
            Role role,
            string password,
            DateTime birthDate,
            Guid? managerId,
            Role creatorRole);

        Task<Employee?> GetEmployeeByIdAsync(Guid id);
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();

        Task<Result<Employee>> UpdateEmployeeAsync(
            Guid id,
            string firstName,
            string lastName,
            string email,
            List<string> phones,
            Role role,
            DateTime birthDate,
            Guid? managerId,
            Role updaterRole);

        Task DeleteEmployeeAsync(Guid id);
    }
}