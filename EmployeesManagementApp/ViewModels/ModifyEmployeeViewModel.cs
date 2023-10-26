using EmployeesManagement.Service;
using EmployeesManagement.Service.DTO;
using EmployeesManagementApp.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using EmployeesManagementApp.Helpers;
using System.Reflection;
using EmployeesManagementApp.Models;
using System.Threading.Channels;

namespace EmployeesManagementApp.ViewModels
{
    public class ModifyEmployeeViewModel
    {
        private readonly IEmployeeService _employeeService;

        public int Id { get; private set; }

        public ICommand SaveEmployeeCommand { get; private set; }
        public List<string> GenderList { get; private set; }
        public List<string> StatusList { get; private set; }

        public AsyncCommand<EmployeeDto, Employee> SaveEmployee { get; private set; }
        public AsyncCommand<EmployeeDto, Employee> LoadEmployee { get; private set; }

        public ModifyEmployeeViewModel(IEmployeeService employeeService, int? employeeId)
        {
            _employeeService = employeeService;
            Id = employeeId ?? 0;
            SaveEmployeeCommand = new RelayCommand(SaveEmployeeMethod, CanSaveEmployee);

            GenderList = StaticContent.Genders;
            StatusList = StaticContent.Statuses;


            LoadEmployee = AsyncCommand.Create<EmployeeDto, Employee>(
                    () => _employeeService.GetByIdAsync(Id));

            SaveEmployee =
                AsyncCommand.Create<EmployeeDto, Employee>(
                    () => _employeeService.SaveAsync(LoadEmployee.Result!.ToDto()));

           if (employeeId.HasValue)
                LoadEmployee.Execute(null);
        }

        private bool CanSaveEmployee(object? obj)
        {
            return true;
        }

        private void SaveEmployeeMethod(object? obj)
        {
            if (LoadEmployee.Result == null ||
                string.IsNullOrWhiteSpace(LoadEmployee.Result.Name) ||
                string.IsNullOrWhiteSpace(LoadEmployee.Result.Email))
                return;

            SaveEmployee.Execute(null);
        }
    }
}
