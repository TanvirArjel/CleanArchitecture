using System.Threading.Tasks;
using BlazorWasmApp.Services;
using BlazorWasmApp.ViewModels.DepartmentsViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorWasmApp.Components.Pages.DepartmentComponents
{
    public partial class DeleteDepartmentComponent
    {
        private readonly DepartmentService _departmentService;
        private readonly NavigationManager _navigationManager;

        public DeleteDepartmentComponent(DepartmentService departmentService, NavigationManager navigationManager)
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
            _navigationManager.NavigateTo("department/department-list");
        }
    }
}
