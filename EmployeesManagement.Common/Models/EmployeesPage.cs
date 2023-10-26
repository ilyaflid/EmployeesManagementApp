using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeesManagement.Common.Models
{
    public class EmployeesPage
    {
        public List<Employee> Employees { get; set; } = new List<Employee>();
        public int Page { get; set; }
        public int? TotalPages { get; set; }
        public int PageSize { get; set; }
    }
}
