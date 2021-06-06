using System.Threading.Tasks;
using BlazorApp.Services;
using BlazorApp.ViewModels.DepartmentsViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Pages.Department
{
    public partial class DepartmentDetails
    {
        [Parameter]
        public int DepartmentId { get; set; }

        [Inject]
        private DepartmentService DepartmentService { get; set; }

        private DepartmentDetailsViewModel DepartmentDetailsModel { get; set; }

        protected override async Task OnInitializedAsync()
        {
            DepartmentDetailsModel = await DepartmentService.GetByIdAsync(DepartmentId);
        }
    }
}
