using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MauiBlazor.Shared.Models;
using MauiBlazor.Shared.Models.EmployeeModels;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace MauiBlazor.Shared.Services
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

        public async Task<List<EmployeeDetailsModel>> GetListAsync()
        {
            PaginatedList<EmployeeDetailsModel> paginatedList = await _httpClient
                .GetFromJsonAsync<PaginatedList<EmployeeDetailsModel>>("v1/employees");
            return paginatedList.Items.ToList();
        }

        public async Task<EmployeeDetailsModel> GetDetailsByIdAsync(Guid employeeId)
        {
            EmployeeDetailsModel employee = await _httpClient.GetFromJsonAsync<EmployeeDetailsModel>($"v1/employees/{employeeId}");
            return employee;
        }

        public async Task<HttpResponseMessage> CreateAsync(CreateEmployeeModel createEmployeeViewModel)
        {
            if (createEmployeeViewModel == null)
            {
                throw new ArgumentNullException(nameof(createEmployeeViewModel));
            }

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("v1/employees", createEmployeeViewModel);
            return response;
        }

        public async Task<HttpResponseMessage> UpdateAsync(UpdateEmployeeModel updateEmployeeViewModel)
        {
            if (updateEmployeeViewModel == null)
            {
                throw new ArgumentNullException(nameof(updateEmployeeViewModel));
            }

            HttpResponseMessage response = await _httpClient
                .PutAsJsonAsync($"v1/employees/{updateEmployeeViewModel.Id}", updateEmployeeViewModel);

            return response;
        }

        public async Task<HttpResponseMessage> DeleteAsync(Guid employeeId)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"v1/employees/{employeeId}");
            return response;
        }
    }
}
