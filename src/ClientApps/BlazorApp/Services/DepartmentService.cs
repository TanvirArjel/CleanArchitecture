using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazorApp.ViewModels.DepartmentsViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace BlazorApp.Services
{
    [SingletonService]
    public class DepartmentService
    {
        private readonly HttpClient _httpClient;

        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Not applicable for constructor")]
        public DepartmentService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("EmployeeManagementApi");
        }

        public async Task<List<DepartmentDetailsViewModel>> GetListAsync()
        {
            List<DepartmentDetailsViewModel> departments = await _httpClient.GetFromJsonAsync<List<DepartmentDetailsViewModel>>("departments");
            return departments;
        }

        public async Task<List<SelectListItem>> GetSelectListAsync(int? selectedDepartment = null)
        {
            List<SelectListItem> departments = await _httpClient.GetFromJsonAsync<List<SelectListItem>>($"departments/select-list?selectedDepartment={selectedDepartment}");
            return departments;
        }

        public async Task<HttpResponseMessage> CreateAsync(CreateDepartmentViewModel createDepartmentViewModel)
        {
            if (createDepartmentViewModel == null)
            {
                throw new ArgumentNullException(nameof(createDepartmentViewModel));
            }

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("departments", createDepartmentViewModel);
            return response;
        }

        public async Task<DepartmentDetailsViewModel> GetByIdAsync(int departmentId)
        {
            DepartmentDetailsViewModel response = await _httpClient.GetFromJsonAsync<DepartmentDetailsViewModel>($"departments/{departmentId}");
            return response;
        }

        public async Task<HttpResponseMessage> UpdateAsync(UpdateDepartmentViewModel updateDepartmentViewModel)
        {
            if (updateDepartmentViewModel == null)
            {
                throw new ArgumentNullException(nameof(updateDepartmentViewModel));
            }

            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"departments/{updateDepartmentViewModel.DepartmentId}", updateDepartmentViewModel);
            return response;
        }

        public async Task<HttpResponseMessage> DeleteAsync(int departmentId)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"departments/{departmentId}");

            return response;
        }
    }
}
