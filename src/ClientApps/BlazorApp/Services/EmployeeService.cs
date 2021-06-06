using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazorApp.Utils;
using BlazorApp.ViewModels.EmployeeViewModels;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace BlazorApp.Services
{
    [SingletonService]
    public class EmployeeService
    {
        private readonly HttpClient _httpClient;

        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Not applicable here")]
        public EmployeeService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("EmployeeManagementApi");
        }

        public async Task<List<EmployeeDetailsViewModel>> GetListAsync()
        {
            PaginatedList<EmployeeDetailsViewModel> paginatedList = await _httpClient.GetFromJsonAsync<PaginatedList<EmployeeDetailsViewModel>>("employees");
            return paginatedList.Items;
        }

        public async Task<EmployeeDetailsViewModel> GetDetailsByIdAsync(int employeeId)
        {
            EmployeeDetailsViewModel employee = await _httpClient.GetFromJsonAsync<EmployeeDetailsViewModel>($"employees/{employeeId}");
            return employee;
        }

        public async Task<HttpResponseMessage> CreateAsync(CreateEmployeeViewModel createEmployeeViewModel)
        {
            if (createEmployeeViewModel == null)
            {
                throw new ArgumentNullException(nameof(createEmployeeViewModel));
            }

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("employees", createEmployeeViewModel);
            return response;
        }

        public async Task<HttpResponseMessage> UpdateAsync(UpdateEmployeeViewModel updateEmployeeViewModel)
        {
            if (updateEmployeeViewModel == null)
            {
                throw new ArgumentNullException(nameof(updateEmployeeViewModel));
            }

            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"employees/{updateEmployeeViewModel.EmployeeId}", updateEmployeeViewModel);

            return response;
        }

        public async Task<HttpResponseMessage> DeleteAsync(int employeeId)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"employees/{employeeId}");
            return response;
        }
    }
}
