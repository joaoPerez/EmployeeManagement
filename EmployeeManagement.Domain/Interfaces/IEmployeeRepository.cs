using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Domain.Interfaces
{
    public interface IEmployeeRepository
    {
        Task AddAsync(Employee employee);
        Task<Employee?> GetByIdAsync(Guid id);
        Task<Employee?> GetByDocNumberAsync(string docNumber);
        Task<IEnumerable<Employee>> GetAllAsync();
        Task UpdateAsync(Employee employee);
        Task DeleteAsync(Employee employee);
        Task<bool> SaveChangesAsync();
    }
}
