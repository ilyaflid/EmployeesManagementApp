using EmployeesManagement.Common.Models;
using System.Reflection;

namespace EmployeesManagement.Service.DTO
{
    public class EmployeeDto : IDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Gender? Gender { get; set; }
        public Status? Status { get; set; }

        public EmployeeDto() { }
        public EmployeeDto(Employee employee)
        {
            Id = employee.Id;
            Name = employee.Name;
            Email = employee.Email;
            Gender = employee.Gender;
            Status = employee.Status;
        }

        public Employee ToEmployee()
        {
            var employee = new Employee();
            employee.Id = Id;
            employee.Name = Name;
            employee.Email = Email;
            employee.Gender = Gender;
            employee.Status = Status;

            return employee;
        }
    }
}