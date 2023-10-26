using EmployeesManagement.Service;
using EmployeesManagementApp.Helpers;
using EmployeesManagementApp.Models;
using EmployeesManagementApp.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EmployeesManagementApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        public MainWindow(IEmployeeService employeeService, ReportService reportService)
        {
            InitializeComponent();
            _mainWindowViewModel = new MainWindowViewModel(employeeService, reportService);
            ItemsPerPage_DropDown.ItemsSource = StaticContent.ItemsPerPageOptions;

            DataContext = _mainWindowViewModel;
            _mainWindowViewModel.EmployeesPage.PropertyChanged += EmployeesPage_PropertyChanged;
        }

        private void EmployeesPage_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_mainWindowViewModel.EmployeesPage.Execution!.IsFaulted)
            {
                MessageBox.Show(_mainWindowViewModel.EmployeesPage.Execution.ErrorMessage, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void Filter_Employee_Click(object sender, RoutedEventArgs e)
        {
            _mainWindowViewModel.SetNameFilter(Filter_Employee_TextBox.Text.Trim());
        }

        private void Filter_Clear_Click(object sender, RoutedEventArgs e)
        {
            Filter_Employee_TextBox.Text = "";
        }

        private void Prev_Page_Button_Click(object sender, RoutedEventArgs e)
        {
            _mainWindowViewModel.DecreasePage();
        }

        private void Next_Page_Button_Click(object sender, RoutedEventArgs e)
        {
            _mainWindowViewModel.IncreasePage();
        }

        private void Delete_Employee_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete the employee?", "Deleting employee", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
                return;
            
            int employeeId = (int)((Button)sender).CommandParameter;
            _mainWindowViewModel.DeleteEmployee(employeeId);
        }
        private void Page_TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (int.TryParse(Page_TextBox.Text.Trim(), out int newPage))
                    _mainWindowViewModel.SetPage(newPage);
            }
        }

        private void ItemsPerPage_DropDown_Changed(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (!comboBox.IsLoaded)
                return;

            int pageSize = (int)comboBox.SelectedValue;
            _mainWindowViewModel.SetPageSize(pageSize);
        }
    }
}
