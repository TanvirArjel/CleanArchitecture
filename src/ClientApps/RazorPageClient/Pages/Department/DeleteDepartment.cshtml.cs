using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPageClient.Services;
using RazorPageClient.ViewModels.DepartmentsViewModels;
using System;
using System.Threading.Tasks;

namespace RazorPageClient.Pages.Department
{
    public class DeleteDepartmentModel : PageModel
    {
        private readonly IDepartmentService _departmentService;

        public DeleteDepartmentModel(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public DepartmentDetailsViewModel DepartmentDetailsViewModel { get; private set; }

        public string ErrorMessage { get; private set; }

        public async Task<IActionResult> OnGetAsync(int departmentId)
        {
            await SetDepartmentViewModelValueAsync(departmentId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int departmentId)
        {
            try
            {
                await _departmentService.DeleteDepartmentAsync(departmentId);
                return RedirectToPage("./DepartmentList");
            }
            catch (Exception)
            {
                ErrorMessage = "There is some problem with service. Please try again. If the problem persists please contact with the system administrator.";
            }

            await SetDepartmentViewModelValueAsync(departmentId);
            return Page();
        }

        private async Task<IActionResult> SetDepartmentViewModelValueAsync(int departmentId)
        {
            DepartmentDetailsViewModel departmentDetailsViewModel = await _departmentService.GetDepartmentAsync(departmentId);
            if (departmentDetailsViewModel == null)
            {
                return NotFound();
            }

            DepartmentDetailsViewModel = departmentDetailsViewModel;

            return new NoContentResult();
        }
    }
}