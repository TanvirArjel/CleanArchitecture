using System.Threading.Tasks;
using BlazorApp.Services;
using BlazorApp.ViewModels.DepartmentsViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Components.Pages.Department
{
    public partial class UpdateDepartment
    {
        private readonly DepartmentService _departmentService;
        private readonly NavigationManager _navigationManager;

        public UpdateDepartment(DepartmentService departmentService, NavigationManager navigationManager)
        {
            _departmentService = departmentService;
            _navigationManager = navigationManager;
        }

        [Parameter]
        public int DepartmentId { get; set; }

        private UpdateDepartmentViewModel UpdateDepartmentModel { get; set; }

        private string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            DepartmentDetailsViewModel departmentDetailsViewModel = await _departmentService.GetByIdAsync(DepartmentId);

            if (departmentDetailsViewModel == null)
            {
                ErrorMessage = "Employee with provided id does not exists!";
            }

            UpdateDepartmentModel = new UpdateDepartmentViewModel()
            {
                DepartmentId = departmentDetailsViewModel.DepartmentId,
                DepartmentName = departmentDetailsViewModel.DepartmentName,
                Description = departmentDetailsViewModel.Description,
            };
        }

        private async Task HandleValidSubmit()
        {
            await _departmentService.UpdateAsync(UpdateDepartmentModel);
            _navigationManager.NavigateTo("department/department-list");
        }
    }
}
