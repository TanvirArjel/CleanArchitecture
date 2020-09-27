using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RazorPageClient.Services;
using RazorPageClient.Utils;
using RazorPageClient.ViewModels.EmployeeViewModels;

namespace RazorPageClient.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly HttpClient _httpClient;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Not applicable here")]
        public EmployeeService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("EmployeeManagementApi");
        }

        private static JsonSerializerOptions JsonSerializerOptions => new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<List<EmployeeDetailsViewModel>> GetEmployeeListAsync()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("employees");
            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                PaginatedList<EmployeeDetailsViewModel> paginatedList = JsonSerializer.Deserialize<PaginatedList<EmployeeDetailsViewModel>>(responseString, JsonSerializerOptions);
                return paginatedList.Items;
            }

            throw new ApplicationException($"{response.ReasonPhrase}: The status code is: {(int)response.StatusCode}");
        }

        public async Task<EmployeeDetailsViewModel> GetEmployeeDetailsAsync(int employeeId)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"employees/{employeeId}");
            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(responseString))
                {
                    return null;
                }

                EmployeeDetailsViewModel employee = JsonSerializer.Deserialize<EmployeeDetailsViewModel>(responseString, JsonSerializerOptions);
                return employee;
            }

            throw new ApplicationException($"{response.ReasonPhrase}: The status code is: {(int)response.StatusCode}");
        }

        public async Task CreateEmployeeAsync(CreateEmployeeViewModel createEmployeeViewModel)
        {
            string jsonStringBody = JsonSerializer.Serialize(createEmployeeViewModel);
            using StringContent content = new StringContent(jsonStringBody, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync("employees", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException($"{response.ReasonPhrase}: The status code is: {(int)response.StatusCode}");
            }
        }

        public async Task UpdateEmplyeeAsync(UpdateEmployeeViewModel updateEmployeeViewModel)
        {
            if (updateEmployeeViewModel == null)
            {
                throw new ArgumentNullException(nameof(updateEmployeeViewModel));
            }

            string jsonStringBody = JsonSerializer.Serialize(updateEmployeeViewModel);
            using StringContent content = new StringContent(jsonStringBody, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PutAsync($"employees/{updateEmployeeViewModel.DepartmentId}", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException($"{response.ReasonPhrase}: The status code is: {(int)response.StatusCode}");
            }
        }

        public async Task DeleteEmployee(int employeeId)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"employees/{employeeId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException($"{response.ReasonPhrase}: The status code is: {(int)response.StatusCode}");
            }
        }
    }
}
