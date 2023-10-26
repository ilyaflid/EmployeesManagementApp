using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeesManagement.Common.Exceptions
{
    public class UnacceptableContentException : Exception
    {
        public UnacceptableContentException(string? message) : base($"Unaccepted content: {message}")
        { }
    }
}
