using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPageClient.Services;
using RazorPageClient.ViewModels.EmployeeViewModels;

namespace RazorPageClient.Pages.Employee
{
    public class EmployeeDetailsModel : PageModel
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeDetailsModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public EmployeeDetailsViewModel EmployeeDetailsViewModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int employeeId)
        {
            EmployeeDetailsViewModel employeeDetailsViewModel = await _employeeService.GetEmployeeDetailsAsync(employeeId);

            if (employeeDetailsViewModel == null)
            {
                return NotFound();
            }

            EmployeeDetailsViewModel = employeeDetailsViewModel;
            return Page();
        }
    }
}