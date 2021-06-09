using System.Threading.Tasks;
using BlazorApp.Services;
using BlazorApp.ViewModels.DepartmentsViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Components.Pages.Department
{
    public partial class DeleteDepartment
    {
        private readonly DepartmentService _departmentService;
        private readonly NavigationManager _navigationManager;

        public DeleteDepartment(DepartmentService departmentService, NavigationManager navigationManager)
        {
            _departmentService = departmentService;
            _navigationManager = navigationManager;
        }

        [Parameter]
        public int DepartmentId { get; set; }

        private DepartmentDetailsViewModel DepartmentDetailsModel { get; set; }

        private string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            DepartmentDetailsModel = await _departmentService.GetByIdAsync(DepartmentId);
        }

        private async Task HandleValidSubmit()
        {
            await _departmentService.DeleteAsync(DepartmentId);
            _navigationManager.NavigateTo("employee/employee-list");
        }
    }
}
