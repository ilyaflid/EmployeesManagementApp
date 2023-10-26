using EmployeesManagement.Service;
using EmployeesManagement.Service.DTO;
using EmployeesManagementApp.Models.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeesManagementApp.Helpers
{
    public class ReportService
    {
        private readonly IEmployeeService _employeeService;
        private const int pageSize = 100;
        private const int numberOfThreads = 10;
        public ReportService(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<string> GetReportFileAsync()
        {
            var reportRecords = await GetReportDataAsync();
            var dataTable = CreateReportTable();
            var reportDataRows = MapReportData(reportRecords, dataTable);

            var sortedRows = reportDataRows.OrderBy(row => row[ReportColumns.EmployeeName]);

            foreach (var dataRow in sortedRows)
                dataTable.Rows.Add(dataRow);

            return GetReport(dataTable);
        }

        private async Task<List<EmployeeDto>> GetReportDataAsync()
        {
            var page = 1;
            List<EmployeeDto> employeesList = new List<EmployeeDto>();

            var retrievedPage = await RetrievePage(page);
            employeesList.AddRange(retrievedPage.Employees);
            var totalPages = retrievedPage.TotalPages;

            while (page < totalPages)
            {
                List<Task<EmployeesPageDto>> tasks = new List<Task<EmployeesPageDto>>();
                
                for (var i = 1; i <= numberOfThreads && page + i <= totalPages; i++)
                    tasks.Add(RetrievePage(page + i));

                while (tasks.Any())
                {
                    var finishedTask = await Task.WhenAny(tasks);
                    tasks.Remove(finishedTask);
                    employeesList.AddRange(finishedTask.Result.Employees);
                }

                page += numberOfThreads;
            }

            return employeesList;
        }

        private async Task<EmployeesPageDto> RetrievePage(int page)
        {
            return await _employeeService.GetAllAsync(null, page, pageSize);
        }

        private DataTable CreateReportTable()
        {
            var dataTable = new DataTable();
            dataTable.Columns.AddRange(new[]
            {
                new DataColumn(ReportColumns.EmployeeId),
                new DataColumn(ReportColumns.EmployeeName),
                new DataColumn(ReportColumns.EmployeeEmail),
                new DataColumn(ReportColumns.EmployeeGender),
                new DataColumn(ReportColumns.EmployeeStatus)
            });
            return dataTable;
        }

        private List<DataRow> MapReportData(List<EmployeeDto> result, DataTable dataTable)
        {
            var rows = new List<DataRow>();

            foreach (var report in result)
            {
                var row = dataTable.NewRow();
                row[ReportColumns.EmployeeId] = report.Id;
                row[ReportColumns.EmployeeName] = report.Name;
                row[ReportColumns.EmployeeEmail] = report.Email;
                row[ReportColumns.EmployeeGender] = report.Gender.ToString();
                row[ReportColumns.EmployeeStatus] = report.Status.ToString();

                rows.Add(row);
            }

            return rows;
        }

        public string GetReport(DataTable dataTable)
        {
            var csvBuilder = new StringBuilder();


            for (var i = 0; i < dataTable.Columns.Count; i++)
            {
                csvBuilder.Append($"{dataTable.Columns[i].ColumnName};");
            }

            csvBuilder.AppendLine();

            for (var i = 0; i < dataTable.Rows.Count; i++)
            {
                for (var j = 0; j < dataTable.Columns.Count; j++)
                {
                    csvBuilder.Append($"{dataTable.Rows[i][j]};");
                }

                csvBuilder.AppendLine();
            }

            var csvReport = csvBuilder.ToString();
            return csvReport;
        }
    }
}
