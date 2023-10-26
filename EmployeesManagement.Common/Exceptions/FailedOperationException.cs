using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeesManagement.Common.Exceptions
{
    public class FailedOperationException : Exception
    {
        public FailedOperationException(string? message) : base($"Operation failed: {message}")
        { }
    }
}
