using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CleanHr.Blazor.Models;
using CleanHr.Blazor.Models.EmployeeModels;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace CleanHr.Blazor.Services;

[SingletonService]
public class EmployeeService
{
    private readonly HttpClient _httpClient;

    public EmployeeService(IHttpClientFactory httpClientFactory)
    {
        httpClientFactory.ThrowIfNull(nameof(httpClientFactory));

        _httpClient = httpClientFactory.CreateClient("EmployeeManagementApi");
    }

    public async Task<List<EmployeeDetailsModel>> GetListAsync()
    {
        PaginatedList<EmployeeDetailsModel> paginatedList = await _httpClient
            .GetFromJsonAsync<PaginatedList<EmployeeDetailsModel>>("v1/employees?pageNumber=1&&pageSize=50");
        return paginatedList.Items.ToList();
    }

    public async Task<EmployeeDetailsModel> GetDetailsByIdAsync(Guid employeeId)
    {
        EmployeeDetailsModel employee = await _httpClient.GetFromJsonAsync<EmployeeDetailsModel>($"v1/employees/{employeeId}");
        return employee;
    }

    public async Task<HttpResponseMessage> CreateAsync(CreateEmployeeModel createEmployeeViewModel)
    {
        ArgumentNullException.ThrowIfNull(createEmployeeViewModel);

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("v1/employees", createEmployeeViewModel);
        return response;
    }

    public async Task<HttpResponseMessage> UpdateAsync(UpdateEmployeeModel updateEmployeeViewModel)
    {
        ArgumentNullException.ThrowIfNull(updateEmployeeViewModel);

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
