using BlazorClientApp.Services;
using BlazorClientApp.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace BlazorClientApp.Pages.Employee
{
    public partial class EmployeeDetails
    {
        [Parameter]
        public int EmployeeId { get; set; }

        [Inject]
        private IEmployeeService EmployeeService { get; set; }

        private EmployeeDetailsViewModel EmployeeDetailsModel { get; set; }

        protected override async Task OnInitializedAsync()
        {
            EmployeeDetailsModel = await EmployeeService.GetEmployeeDetailsAsync(EmployeeId);
        }
    }
}
