using EmployeesManagement.Common;
using EmployeesManagement.Common.Exceptions;
using EmployeesManagement.Common.Models;
using System.Text;

namespace EmployeesManagement.Tests
{
    public class ApiDataRepositoryTests
    {
        private ApiDataRepository _repository;
        private const string API_ADDRESS = "https://gorest.co.in/public/v2";
        private const string API_KEY = "0bf7fb56e6a27cbcadc402fc2fce8e3aa9ac2b40d4190698eb4e8df9284e2023";
        public ApiDataRepositoryTests()
        {
            _repository = new ApiDataRepository(API_ADDRESS, API_KEY);
        }

        [Fact]
        public async Task GetAllTests()
        {
            var results = await _repository.GetAllAsync();

            Assert.NotNull(results);
            Assert.NotEmpty(results.Employees);
            Assert.NotNull(results.TotalPages);
            Assert.Equal(1, results.Page);
            Assert.Equal(10, results.PageSize);
        }

        [Fact]
        public async Task GetAll_SpecificPage_Tests()
        {
            var results = await _repository.GetAllAsync(2, 20);

            Assert.NotNull(results);
            Assert.NotEmpty(results.Employees);
            Assert.NotNull(results.TotalPages);
            Assert.Equal(2, results.Page);
            Assert.Equal(20, results.PageSize);
        }

        [Fact]
        public async Task GetAll_OverflowPage_Tests()
        {
            var requestedPage = 2000;
            var results = await _repository.GetAllAsync(requestedPage, 10);

            Assert.NotNull(results);
            Assert.Empty(results.Employees);
            Assert.NotNull(results.TotalPages);
            Assert.True(results.TotalPages < requestedPage);
            Assert.Equal(requestedPage, results.Page);
            Assert.Equal(10, results.PageSize);
        }

        [Fact]
        public async Task GetAll_Filtered_Tests()
        {
            var nameFilter = "jane";
            var results = await _repository.GetAllAsync(name: nameFilter);

            Assert.NotNull(results);
            Assert.NotEmpty(results.Employees);
            foreach (var employee in results.Employees)
                Assert.Contains(nameFilter, employee.Name);
            Assert.NotNull(results.TotalPages);
            Assert.Equal(1, results.Page);
            Assert.Equal(10, results.PageSize);
        }

        [Fact]
        public void GetSpecific_WrongId_Tests()
        {
            var employeeId = 0;

            Assert.ThrowsAsync<EmployeeNotFoundException>(async () => await _repository.GetAsync(employeeId));
        }

        [Fact]
        public async Task GetSpecific_CorrectId_Tests()
        {
            var employeeId = 5452170;
            var result = await _repository.GetAsync(employeeId);

            Assert.NotNull(result);
            Assert.Equal(employeeId, result.Id);
        }

        [Fact]
        public async Task CreateEmployee_Tests()
        {
            var index = new Random().Next(100);
            var employee = new Employee();
            employee.Name = $"Test name {index}";
            employee.Email = $"test{index}@testname.domain";
            employee.Gender = Gender.Male;
            employee.Status = Status.Inactive;

            await _repository.CreateAsync(employee);

            Assert.True(employee.Id > 0);
        }

        [Fact]
        public async Task CreateEmployee_DoubleCreation_Tests()
        {
            var index = new Random().Next(100);
            var employee = new Employee();
            employee.Name = $"Test name {index}";
            employee.Email = $"test{index}@testname.domain";
            employee.Gender = Gender.Male;
            employee.Status = Status.Inactive;

            await _repository.CreateAsync(employee);
            Assert.True(employee.Id > 0);

            await Assert.ThrowsAsync<FailedOperationException>(async () => await _repository.CreateAsync(employee));
        }

        [Fact]
        public async Task UpdateEmployee_NonExistingItem_Tests()
        {
            var index = new Random().Next(100);
            var employee = new Employee();
            employee.Name = $"Test name {index}";
            employee.Email = $"test{index}@testname.domain";
            employee.Gender = Gender.Male;
            employee.Status = Status.Inactive;

            await Assert.ThrowsAsync<EmployeeNotFoundException>(async () => await _repository.UpdateAsync(employee));
        }

        [Fact]
        public async Task UpdateEmployee_Tests()
        {
            var index = new Random().Next(100);
            var employee = new Employee();
            employee.Name = $"Test name {index}";
            employee.Email = $"test{index}@testname.domain";
            employee.Gender = Gender.Male;
            employee.Status = Status.Inactive;

            await _repository.CreateAsync(employee);
            employee.Status = Status.Active;

            await _repository.UpdateAsync(employee);
            var result = await _repository.GetAsync(employee.Id);

            Assert.NotNull(result);
            Assert.Equal(Status.Active, result.Status);
        }

        [Fact]
        public async Task DeleteEmployee_Tests()
        {
            var index = new Random().Next(100);
            var employee = new Employee();
            employee.Name = $"Test name {index}";
            employee.Email = $"test{index}@testname.domain";
            employee.Gender = Gender.Male;
            employee.Status = Status.Inactive;

            await _repository.CreateAsync(employee);

            await _repository.DeleteAsync(employee.Id);

            await Assert.ThrowsAsync<EmployeeNotFoundException>(async () => await _repository.GetAsync(employee.Id));
        }

        [Fact]
        public async Task DeleteEmployee_NonExistingItem_Tests()
        {
            await Assert.ThrowsAsync<EmployeeNotFoundException>(async () => await _repository.DeleteAsync(0));
        }
    }
}