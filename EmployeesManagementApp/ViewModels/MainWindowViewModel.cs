using EmployeesManagement.Service;
using EmployeesManagement.Service.DTO;
using EmployeesManagementApp.Commands;
using EmployeesManagementApp.Helpers;
using EmployeesManagementApp.Models;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace EmployeesManagementApp.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly IEmployeeService _employeeService;
        private readonly ReportService _reportService;

        private int _page = 1;
        private int _pageSize = 10;
        private string? _nameFilter = null;
        private bool _isLoading = false;

        public event PropertyChangedEventHandler? PropertyChanged;
        public ICommand ShowModifyEmployeeDialog { get; private set; }
        public ICommand ExportItemsCommand { get; private set; }
        public bool IsLoading { 
            get => _isLoading; 
            private set {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            } 
        }
        public AsyncCommand<EmployeesPageDto, EmployeesPage> EmployeesPage { get; private set; }
        public MainWindowViewModel(IEmployeeService employeeService, ReportService reportService)
        {
            _employeeService = employeeService;
            _reportService = reportService;
            ShowModifyEmployeeDialog = new RelayCommand(ShowModifyEmployeeWindow, CanShowModifyEmployeeWindow);
            ExportItemsCommand = new RelayCommand(ExportItems, CanExportItems);
            EmployeesPage = AsyncCommand.Create<EmployeesPageDto, EmployeesPage>(
                () => _employeeService.GetAllAsync(_nameFilter, _page, _pageSize));

            EmployeesPage.Execute(null);
            EmployeesPage.PropertyChanged += EmployeesPage_PropertyChanged;
        }

        private void EmployeesPage_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (EmployeesPage.Execution == null)
                return;

            IsLoading = EmployeesPage.Execution.IsNotCompleted;
        }

        private bool CanShowModifyEmployeeWindow(object? obj)
        {
            return true;
        }

        private void ShowModifyEmployeeWindow(object? obj)
        {
            var _modifyEmployeeForm = new ModifyEmployee(_employeeService, (int?)obj);
            _modifyEmployeeForm.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            _modifyEmployeeForm.Saved += _modifyEmployeeForm_Closed;
            _modifyEmployeeForm.ShowDialog();
        }

        private bool CanExportItems(object? obj)
        {
            return true;
        }

        private void ExportItems(object? obj)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.AddExtension = true;
            saveFileDialog.CreatePrompt = true;
            saveFileDialog.FileName = "Employees export.csv";
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
            saveFileDialog.FileOk += SaveFileDialog_FileOk;
            saveFileDialog.ShowDialog();
        }

        private void SaveFileDialog_FileOk(object? sender, CancelEventArgs e)
        {
            IsLoading = true;
            var fileName = ((SaveFileDialog)sender!).FileName;

            _reportService.GetReportFileAsync()
                .ContinueWith(t => {
                    if (t.IsFaulted)
                        MessageBox.Show(t.Exception?.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        File.WriteAllText(fileName, t.Result);

                    IsLoading = false;
                });
        }

        private void _modifyEmployeeForm_Closed(object? sender, EventArgs e)
        {
            EmployeesPage.Execute(null);
        }

        public void SetPage(int page)
        {
            if (page <= 0 || page > EmployeesPage?.Result?.NumberOfPages)
                return;

            if (_page == page)
                return;

            _page = page;
            OnPropertyChanged();
            EmployeesPage!.Execute(null);
        }

        public void IncreasePage() => SetPage(_page + 1);
        public void DecreasePage() => SetPage(_page - 1);

        public void SetPageSize(int pageSize)
        {
            if (pageSize <= 0)
                return;

            _pageSize = pageSize;
            _page = 1;
            OnPropertyChanged();
            EmployeesPage.Execute(null);
        }

        public void SetNameFilter(string nameFilter)
        {
            if (nameFilter.Equals(_nameFilter, StringComparison.InvariantCultureIgnoreCase))
                return;

            _nameFilter = string.IsNullOrWhiteSpace(nameFilter) ? null : nameFilter.Trim();
            _page = 1;
            OnPropertyChanged();
            EmployeesPage.Execute(null);
        }

        public void DeleteEmployee(int employeeId)
        {
            IsLoading = true;
            _employeeService.DeleteAsync(employeeId)
                .ContinueWith(t => {
                    IsLoading = false;

                    if (t.IsFaulted)
                        MessageBox.Show(t.Exception?.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        EmployeesPage.Execute(null);
                });
        }
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
