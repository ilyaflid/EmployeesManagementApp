using EmployeesManagement.Common.Exceptions;
using EmployeesManagement.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace EmployeesManagement.Common
{
    public class ApiDataRepository : IDataRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiAddress;

        private const string USERS_PATH = "users";
        private const string PAGE_HEADER = "X-Pagination-Page";
        private const string TOTAL_HEADER = "X-Pagination-Total";

        public ApiDataRepository(string apiAddress, string apiKey)
        {
            _httpClient = new HttpClient();
            _apiAddress = apiAddress.TrimEnd('/');
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }

        public async Task<EmployeesPage> GetAllAsync(int page=1, int pageSize = 10, string? name = null, CancellationToken cancellationToken = default)
        {
            var requestUri = $"{_apiAddress}/{USERS_PATH}?page={page}&per_page={pageSize}";
            if (name != null)
                requestUri += $"&name={name}";

            using (var response = await _httpClient.GetAsync(requestUri))
            {
                if (response.IsSuccessStatusCode)
                {
                    var resultContent = await response.Content.ReadFromJsonAsync<List<Employee>>();

                    if (resultContent == null)
                        throw new InvalidCastException($"Can't cast the response message (page {page}, page size {pageSize}, name filter {name}");

                    var employeesPage = new EmployeesPage();
                    employeesPage.Employees = resultContent;
                    employeesPage.PageSize = pageSize;

                    if (response.Headers.TryGetValues(PAGE_HEADER, out var pageNumberValues) &&
                        int.TryParse(pageNumberValues?.FirstOrDefault(), out int pageNumberResponse))
                        employeesPage.Page = pageNumberResponse;
                    else
                        employeesPage.Page = page;

                    if (response.Headers.TryGetValues(TOTAL_HEADER, out var totalItemsValues) &&
                        int.TryParse(totalItemsValues?.FirstOrDefault(), out int totalItemsResponse))
                        employeesPage.TotalPages = totalItemsResponse % pageSize == 0 ?
                            totalItemsResponse / pageSize :
                            totalItemsResponse / pageSize + 1;

                    return employeesPage;
                }

                throw new FailedOperationException(response.ReasonPhrase);
            }   
        }
        public async Task<Employee?> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            var requestUri = $"{_apiAddress}/{USERS_PATH}/{id}";
            using (var response = await _httpClient.GetAsync(requestUri))
            {
                if (response.IsSuccessStatusCode)
                {
                    var resultContent = await response.Content.ReadFromJsonAsync<Employee>();

                    if (resultContent == null)
                        throw new InvalidCastException($"Can't cast the response message (id: {id})");

                    return resultContent;
                }

                throw response.StatusCode switch
                {
                    HttpStatusCode.NotFound => new EmployeeNotFoundException(id),
                    _ => new FailedOperationException(response.ReasonPhrase)
                };
            }
        }
        public async Task CreateAsync(Employee employee, CancellationToken cancellationToken = default)
        {
            var requestUri = $"{_apiAddress}/{USERS_PATH}";

            using (var response = await _httpClient.PostAsync(requestUri, JsonContent.Create(employee)))
            {
                if (!response.IsSuccessStatusCode)
                    throw response.StatusCode switch
                    {
                        HttpStatusCode.UnprocessableEntity => new UnacceptableContentException(response.ReasonPhrase),
                        _ => new FailedOperationException(response.ReasonPhrase)
                    };
                if (int.TryParse(response.Headers.Location?.Segments.LastOrDefault(), out int id))
                    employee.Id = id;
            }
        }
        public async Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default)
        {
            var requestUri = $"{_apiAddress}/{USERS_PATH}/{employee.Id}";
            using (var response = await _httpClient.PutAsync(requestUri, JsonContent.Create(employee)))
            {
                if (!response.IsSuccessStatusCode) {
                    throw response.StatusCode switch
                    {
                        HttpStatusCode.NotFound => new EmployeeNotFoundException(employee.Id),
                        _ => new FailedOperationException(response.ReasonPhrase)
                    };
                }
            }
        }
        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var requestUri = $"{_apiAddress}/{USERS_PATH}/{id}";
            using (var response = await _httpClient.DeleteAsync(requestUri))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw response.StatusCode switch
                    {
                        HttpStatusCode.NotFound => new EmployeeNotFoundException(id),
                        _ => new FailedOperationException(response.ReasonPhrase)
                    };
                }
            }
        }
    }
}
