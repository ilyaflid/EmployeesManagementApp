using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeesManagement.Service.DTO;

namespace EmployeesManagement.Service
{
    public interface IEmployeeService
    {
        Task<EmployeesPageDto> GetAllAsync(string? nameFilter, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<EmployeeDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<EmployeeDto> SaveAsync(EmployeeDto employeeDto, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
