using System.Threading.Tasks;
using BlazorApp.Services;
using BlazorApp.ViewModels.DepartmentsViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Pages.Department
{
    public partial class DepartmentDetails
    {
        private readonly DepartmentService _departmentService;

        public DepartmentDetails(DepartmentService departmentService)
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
