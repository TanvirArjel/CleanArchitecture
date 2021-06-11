using System.Threading.Tasks;
using BlazorWasmApp.Services;
using BlazorWasmApp.ViewModels.DepartmentsViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorWasmApp.Components.Pages.DepartmentComponents
{
    public partial class DepartmentDetailsComponent
    {
        private readonly DepartmentService _departmentService;

        public DepartmentDetailsComponent(DepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [Parameter]
        public int DepartmentId { get; set; }

        private DepartmentDetailsViewModel DepartmentDetailsModel { get; set; }

        protected override async Task OnInitializedAsync()
        {
            DepartmentDetailsModel = await _departmentService.GetByIdAsync(DepartmentId);
        }
    }
}
