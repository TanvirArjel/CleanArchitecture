using BlazorClientApp.Services;
using BlazorClientApp.ViewModels.DepartmentsViewModels;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace BlazorClientApp.Pages.Department
{
    public partial class UpdateDepartment
    {
        [Parameter]
        public int DepartmentId { get; set; }

        [Inject]
        private IDepartmentService DepartmentService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        private UpdateDepartmentViewModel UpdateDepartmentModel { get; set; }

        private string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            DepartmentDetailsViewModel departmentDetailsViewModel = await DepartmentService.GetDepartmentAsync(DepartmentId);

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
            await DepartmentService.UpdateDepartmentAsync(UpdateDepartmentModel);
            NavigationManager.NavigateTo("department/department-list");
        }
    }
}
