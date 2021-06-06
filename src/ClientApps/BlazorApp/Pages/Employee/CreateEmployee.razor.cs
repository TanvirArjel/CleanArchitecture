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
        private EmployeeService EmployeeService { get; set; }

        [Inject]
        private DepartmentService DepartmentService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        private CreateEmployeeViewModel CreateEmployeeViewModel { get; set; } = new CreateEmployeeViewModel();

        private SelectList DepartmentSelectList { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());

        protected override async Task OnInitializedAsync()
        {
            SelectList items = await DepartmentService.GetSelectListAsync();
            DepartmentSelectList = items;
        }

        private async Task HandleValidSubmit()
        {
            await EmployeeService.CreateAsync(CreateEmployeeViewModel);
            NavigationManager.NavigateTo("employee/employee-list");
        }
    }
}
