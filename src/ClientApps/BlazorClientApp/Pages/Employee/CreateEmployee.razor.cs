using BlazorClientApp.Services;
using BlazorClientApp.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorClientApp.Pages.Employee
{
    public partial class CreateEmployee
    {
        [Inject]
        private IDepartmentService DepartmentService { get; set; }

        private CreateEmployeeViewModel CreateEmployeeViewModel { get; set; } = new CreateEmployeeViewModel();

        private SelectList DepartmentSelectList { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());

        protected override async Task OnInitializedAsync()
        {
            SelectList items = await DepartmentService.GetDepartmentSelectListAsync();
            DepartmentSelectList = items;
        }

        private void HandleValidSubmit()
        {
            Console.WriteLine("OnValidSubmit");
        }
    }
}
