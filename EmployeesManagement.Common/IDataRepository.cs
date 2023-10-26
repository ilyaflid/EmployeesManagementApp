using System.Threading;
using EmployeesManagement.Common.Models;

namespace EmployeesManagement.Common
{
    public interface IDataRepository
    {
        Task<EmployeesPage> GetAllAsync(int page, int pageSize, string? name, CancellationToken cancellationToken = default);
        Task<Employee?> GetAsync(int id, CancellationToken cancellationToken = default);
        Task CreateAsync(Employee employee, CancellationToken cancellationToken = default);
        Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}