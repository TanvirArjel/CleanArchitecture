using RazorPageClient.ViewModels.EmployeeViewModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RazorPageClient.Services.EmployeeService
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
            HttpResponseMessage response = await _httpClient.GetAsync("employee/get-employee-list");
            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                List<EmployeeDetailsViewModel> employees = JsonSerializer.Deserialize<List<EmployeeDetailsViewModel>>(responseString, JsonSerializerOptions);
                return employees;
            }

            throw new ApplicationException($"{response.ReasonPhrase}: The status code is: {(int)response.StatusCode}");
        }

        public async Task<EmployeeDetailsViewModel> GetEmployeeDetailsAsync(int employeeId)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"employee/get-employee-details/{employeeId}");
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
            string jsonStringBody = JsonSerializer.Serialize<CreateEmployeeViewModel>(createEmployeeViewModel);
            using StringContent content = new StringContent(jsonStringBody, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync("employee/create-employee", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException($"{response.ReasonPhrase}: The status code is: {(int)response.StatusCode}");
            }
        }

        public async Task UpdateEmplyeeAsync(UpdateEmployeeViewModel updateEmployeeViewModel)
        {
            string jsonStringBody = JsonSerializer.Serialize<UpdateEmployeeViewModel>(updateEmployeeViewModel);
            using StringContent content = new StringContent(jsonStringBody, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PutAsync("employee/update-employee", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException($"{response.ReasonPhrase}: The status code is: {(int)response.StatusCode}");
            }
        }

        public async Task DeleteEmployee(int employeeId)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"employee/delete-employee/{employeeId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException($"{response.ReasonPhrase}: The status code is: {(int)response.StatusCode}");
            }
        }
    }
}
