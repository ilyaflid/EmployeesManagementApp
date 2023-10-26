using EmployeesManagement.Common;
using EmployeesManagement.Common.Exceptions;
using EmployeesManagement.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeesManagement.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IDataRepository _repository;
        public EmployeeService(IDataRepository repository) => _repository = repository;
        public async Task<EmployeeDto> SaveAsync(EmployeeDto employeeDto, CancellationToken cancellationToken = default)
        {
            var employee = employeeDto.ToEmployee();
            if (employee.Id == 0)
                await _repository.CreateAsync(employee);
            else
                await _repository.UpdateAsync(employee);

            return new EmployeeDto(employee);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<EmployeesPageDto> GetAllAsync(string? nameFilter, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var employees = await _repository.GetAllAsync(page, pageSize, nameFilter);
            return new EmployeesPageDto(employees);
        }

        public async Task<EmployeeDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var employee = await _repository.GetAsync(id);
            if (employee == null)
                throw new EmployeeNotFoundException(id);

            return new EmployeeDto(employee);
        }
    }
}
