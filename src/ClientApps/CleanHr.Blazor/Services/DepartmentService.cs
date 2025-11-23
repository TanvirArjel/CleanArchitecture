using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CleanHr.Blazor.Models;
using CleanHr.Blazor.Models.DepartmentModels;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace CleanHr.Blazor.Services;

[SingletonService]
public class DepartmentService
{
    private readonly HttpClient _httpClient;

    public DepartmentService(IHttpClientFactory httpClientFactory)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        _httpClient = httpClientFactory.CreateClient("EmployeeManagementApi");
    }

    public async Task<HttpResponseMessage> GetListAsync()
    {
        HttpResponseMessage httpResponse = await _httpClient.GetAsync("v1/departments");
        return httpResponse;
    }

    public async Task<List<SelectListItem>> GetSelectListAsync(Guid? selectedDepartment = null)
    {
        List<SelectListItem> departments = await _httpClient
            .GetFromJsonAsync<List<SelectListItem>>($"v1/departments/select-list?selectedDepartment={selectedDepartment}");
        return departments;
    }

    public async Task<HttpResponseMessage> CreateAsync(CreateDepartmentModel createDepartmentViewModel)
    {
        createDepartmentViewModel.ThrowIfNull(nameof(createDepartmentViewModel));

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("v1/departments", createDepartmentViewModel);
        return response;
    }

    public async Task<DepartmentDetailsModel> GetByIdAsync(Guid departmentId)
    {
        departmentId.ThrowIfEmpty(nameof(departmentId));

        DepartmentDetailsModel response = await _httpClient.GetFromJsonAsync<DepartmentDetailsModel>($"v1/departments/{departmentId}");
        return response;
    }

    public async Task<HttpResponseMessage> UpdateAsync(UpdateDepartmentModel updateDepartmentViewModel)
    {
        updateDepartmentViewModel.ThrowIfNull(nameof(updateDepartmentViewModel));

        HttpResponseMessage response = await _httpClient
            .PutAsJsonAsync($"v1/departments/{updateDepartmentViewModel.Id}", updateDepartmentViewModel);
        return response;
    }

    public async Task<HttpResponseMessage> DeleteAsync(Guid departmentId)
    {
        departmentId.ThrowIfEmpty(nameof(departmentId));

        HttpResponseMessage response = await _httpClient.DeleteAsync($"v1/departments/{departmentId}");

        return response;
    }
}
