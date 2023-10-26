using EmployeesManagement.Common.Models;
using EmployeesManagement.Service.DTO;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeesManagementApp.Models
{
    public class Employee : ICastable<Employee, EmployeeDto>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public string? Status { get; set; }

        public Employee() { }
        public Employee(EmployeeDto employeeDto)
        {
            Id = employeeDto.Id;
            Name = employeeDto.Name;
            Email = employeeDto.Email;
            Gender = employeeDto.Gender.ToString();
            Status = employeeDto.Status.ToString();
        }

        public Employee FromDto(EmployeeDto employeeDto)
        {
            Id = employeeDto.Id;
            Name = employeeDto.Name;
            Email = employeeDto.Email;
            Gender = employeeDto.Gender.ToString();
            Status = employeeDto.Status.ToString();
            return this;
        }

        public EmployeeDto ToDto()
        {
            var gender = Gender switch
            {
                "Male" => EmployeesManagement.Common.Models.Gender.Male,
                "Female" => EmployeesManagement.Common.Models.Gender.Female,
                "Non-Binary" => EmployeesManagement.Common.Models.Gender.NonBinary,
                _ => EmployeesManagement.Common.Models.Gender.NotDefined
            };

            var status = Status switch
            {
                "Active" => EmployeesManagement.Common.Models.Status.Active,
                "Inactive" => EmployeesManagement.Common.Models.Status.Inactive,
                _ => EmployeesManagement.Common.Models.Status.NotDefined
            };

            return new EmployeeDto()
            {
                Id = Id,
                Name = Name,
                Email = Email,
                Gender = gender,
                Status = status
            };

        }
    }

    public class EmployeesPage : ICastable<EmployeesPage, EmployeesPageDto>
    {
        public List<Employee> Employees { get; private set; } = new List<Employee>();
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int NumberOfPages { get; set; } = 1;

        public EmployeesPage() { }
        public EmployeesPage(EmployeesPageDto employeesPageDto)
        {
            Employees = employeesPageDto.Employees?.Select(t => new Employee(t)).ToList()
                ?? new List<Employee>();
            Page = employeesPageDto.Page;
            PageSize = employeesPageDto.PageSize;
            NumberOfPages = employeesPageDto.TotalPages ?? 1;
        }

        public EmployeesPage FromDto(EmployeesPageDto employeesPageDto)
        {
            Employees = employeesPageDto.Employees?.Select(t => new Employee(t)).ToList()
                ?? new List<Employee>();
            Page = employeesPageDto.Page;
            PageSize = employeesPageDto.PageSize;
            NumberOfPages = employeesPageDto.TotalPages ?? 1;
            return this;
        }
    }

    public interface ICastable<T, U>
    {
        T FromDto(U dto);
    }
}
