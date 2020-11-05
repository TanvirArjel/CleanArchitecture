using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPageApp.Services;
using RazorPageApp.ViewModels.DepartmentsViewModels;

namespace RazorPageApp.Pages.Department
{
    public class CreateDepartmentModel : PageModel
    {
        private readonly IDepartmentService _departmentService;

        public CreateDepartmentModel(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [BindProperty]
        public CreateDepartmentViewModel CreateDepartmentViewModel { get; set; }

        // [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Warning not applicable for Razor Pages")]
        public static void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _departmentService.CreateDepartmentAsync(CreateDepartmentViewModel);
                    return RedirectToPage("./DepartmentList");
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "There is some problem with the service. Please try again. If the problem persist, Please contract with system administrator.");
                }
            }

            return Page();
        }
    }
}