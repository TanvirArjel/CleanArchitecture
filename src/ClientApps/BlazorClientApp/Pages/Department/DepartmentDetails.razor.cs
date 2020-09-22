using System.Threading.Tasks;
using BlazorClientApp.Services;
using BlazorClientApp.ViewModels.DepartmentsViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorClientApp.Pages.Department
{
    public partial class DepartmentDetails
    {
        [Parameter]
        public int DepartmentId { get; set; }

        [Inject]
        private IDepartmentService DepartmentService { get; set; }

        private DepartmentDetailsViewModel DepartmentDetailsModel { get; set; }

        protected override async Task OnInitializedAsync()
        {
            DepartmentDetailsModel = await DepartmentService.GetDepartmentAsync(DepartmentId);
        }
    }
}
