using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Services;
using BlazorApp.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlazorApp.Pages.Employee
{
    public partial class CreateEmployee
    {
        [Inject]
        private IEmployeeService EmployeeService { get; set; }

        [Inject]
        private IDepartmentService DepartmentService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        private CreateEmployeeViewModel CreateEmployeeViewModel { get; set; } = new CreateEmployeeViewModel();

        private SelectList DepartmentSelectList { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());

        protected override async Task OnInitializedAsync()
        {
            SelectList items = await DepartmentService.GetDepartmentSelectListAsync();
            DepartmentSelectList = items;
        }

        private async Task HandleValidSubmit()
        {
            await EmployeeService.CreateEmployeeAsync(CreateEmployeeViewModel);
            NavigationManager.NavigateTo("employee/employee-list");
        }
    }
}
