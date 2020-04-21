using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPageClient.Services;
using RazorPageClient.ViewModels.EmployeeViewModels;

namespace RazorPageClient.Pages.Employee
{
    public class DeleteEmployeeModel : PageModel
    {
        private readonly IEmployeeService _employeeService;

        public DeleteEmployeeModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public EmployeeDetailsViewModel EmployeeDetailsViewModel { get; private set; }

        public string ErrorMessage { get; private set; }

        public async Task<IActionResult> OnGetAsync(int employeeId)
        {
            await SetEmployeeDetailsViewModelAsync(employeeId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int employeeId)
        {
            try
            {
                await _employeeService.DeleteEmployee(employeeId);
                return RedirectToPage("./EmployeeList");
            }
            catch (Exception)
            {
                ErrorMessage = "There is some problem with the service. Please try again. If the problem persist, Please contract with system administrator.";
            }

            await SetEmployeeDetailsViewModelAsync(employeeId);
            return Page();
        }

        private async Task<IActionResult> SetEmployeeDetailsViewModelAsync(int employeeId)
        {
            EmployeeDetailsViewModel employeeDetailsViewModel = await _employeeService.GetEmployeeDetailsAsync(employeeId);

            if (employeeDetailsViewModel == null)
            {
                return NotFound();
            }

            EmployeeDetailsViewModel = employeeDetailsViewModel;

            return new NoContentResult();
        }
    }
}