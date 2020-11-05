using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RazorPageApp.Services;
using RazorPageApp.ViewModels.EmployeeViewModels;

namespace RazorPageApp.Pages.Employee
{
    public class CreateEmployeeModel : PageModel
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;

        public CreateEmployeeModel(IEmployeeService employeeService, IDepartmentService departmentService)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
        }

        public SelectList DepartmentSelectList { get; set; }

        [BindProperty]
        public CreateEmployeeViewModel CreateEmployeeViewModel { get; set; }

        public async Task OnGetAsync()
        {
            SelectList items = await _departmentService.GetDepartmentSelectListAsync();
            DepartmentSelectList = items;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _employeeService.CreateEmployeeAsync(CreateEmployeeViewModel);
                    return RedirectToPage("./EmployeeList");
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "There is some problem with the service. Please try again. If the problem persist, Please contract with system administrator.");
                }
            }

            SelectList items = await _departmentService.GetDepartmentSelectListAsync(CreateEmployeeViewModel.DepartmentId);
            DepartmentSelectList = items;
            return Page();
        }
    }
}