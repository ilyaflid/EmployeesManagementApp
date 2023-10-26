using EmployeesManagement.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeesManagementApp.Models
{
    public static class StaticContent
    {
        public static List<string> Genders = 
            new List<string>() {
                Gender.Male.ToString(),
                Gender.Female.ToString(),
                Gender.NonBinary.ToString()
            };

        public static List<string> Statuses =
            new List<string>() {
                Status.Active.ToString(),
                Status.Inactive.ToString()
            };

        public static List<int> ItemsPerPageOptions =
            new List<int>() { 10, 20, 50, 100 };
    }
}
