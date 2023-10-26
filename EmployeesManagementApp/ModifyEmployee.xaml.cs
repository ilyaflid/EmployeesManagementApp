using EmployeesManagement.Service;
using EmployeesManagementApp.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EmployeesManagementApp
{
    /// <summary>
    /// Interaction logic for ModifyEmployee.xaml
    /// </summary>
    public partial class ModifyEmployee : Window
    {
        private readonly ModifyEmployeeViewModel _modifyEmployeeModel;
        public delegate void EmployeeSavedEventHandler(object? sender, EventArgs e);
        public event EmployeeSavedEventHandler Saved;
        public ModifyEmployee(IEmployeeService employeeService, int? employeeId)
        {
            InitializeComponent();
            _modifyEmployeeModel = new ModifyEmployeeViewModel(employeeService, employeeId);
            DataContext = _modifyEmployeeModel;
            _modifyEmployeeModel.SaveEmployee.PropertyChanged += SaveEmployee_PropertyChanged;
            _modifyEmployeeModel.LoadEmployee.PropertyChanged += LoadEmployee_PropertyChanged;
        }

        private void LoadEmployee_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_modifyEmployeeModel.LoadEmployee.Execution!.IsFaulted)
            {
                MessageBox.Show(_modifyEmployeeModel.LoadEmployee.Execution.ErrorMessage, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void SaveEmployee_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_modifyEmployeeModel.SaveEmployee.Execution!.IsCompleted)
            {
                if (_modifyEmployeeModel.SaveEmployee.Execution!.IsFaulted)
                {
                    MessageBox.Show(_modifyEmployeeModel.SaveEmployee.Execution.ErrorMessage, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (Saved != null)
                    Saved(this, e);

                Close();
            }
        }
    }
}
