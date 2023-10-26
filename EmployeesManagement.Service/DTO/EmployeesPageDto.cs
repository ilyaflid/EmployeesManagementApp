using EmployeesManagement.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeesManagement.Service.DTO
{
    public class EmployeesPageDto : IDto
    {
        public List<EmployeeDto> Employees { get; set; } = new List<EmployeeDto>();
        public int Page { get; set; }
        public int? TotalPages { get; set; }
        public int PageSize { get; set; }

        public EmployeesPageDto(EmployeesPage employeesPage)
        {
            Employees = employeesPage.Employees.Select(t => new EmployeeDto(t)).ToList();
            Page = employeesPage.Page;
            TotalPages = employeesPage.TotalPages;
            PageSize = employeesPage.PageSize;
        }
    }
}
