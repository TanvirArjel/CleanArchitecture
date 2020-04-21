using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RazorPageClient.Services;
using RazorPageClient.ViewModels.EmployeeViewModels;

namespace RazorPageClient.Pages.Employee
{
    public class UpdateEmployeeModel : PageModel
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;

        public UpdateEmployeeModel(IEmployeeService employeeService, IDepartmentService departmentService)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
        }

        public SelectList DepartmentSelectList { get; set; }

        [BindProperty]
        public UpdateEmployeeViewModel UpdateEmployeeViewModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int employeeId)
        {
            EmployeeDetailsViewModel employeeDetailsViewModel = await _employeeService.GetEmployeeDetailsAsync(employeeId);

            if (employeeDetailsViewModel == null)
            {
                return NotFound();
            }

            UpdateEmployeeViewModel = new UpdateEmployeeViewModel()
            {
                EmployeeId = employeeDetailsViewModel.EmployeeId,
                EmployeeName = employeeDetailsViewModel.EmployeeName,
                DepartmentId = employeeDetailsViewModel.DepartmentId,
                DateOfBirth = employeeDetailsViewModel.DateOfBirth,
                Email = employeeDetailsViewModel.Email,
                PhoneNumber = employeeDetailsViewModel.PhoneNumber
            };

            SelectList items = await _departmentService.GetDepartmentSelectListAsync(employeeDetailsViewModel.DepartmentId);
            DepartmentSelectList = items;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _employeeService.UpdateEmplyeeAsync(UpdateEmployeeViewModel);
                    return RedirectToPage("./EmployeeList");
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "There is some problem with the service. Please try again. If the problem persist, Please contract with system administrator.");
                }
            }

            SelectList items = await _departmentService.GetDepartmentSelectListAsync(UpdateEmployeeViewModel.DepartmentId);
            DepartmentSelectList = items;
            return Page();
        }
    }
}