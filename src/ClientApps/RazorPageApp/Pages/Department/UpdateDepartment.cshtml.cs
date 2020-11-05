using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPageApp.Services;
using RazorPageApp.ViewModels.DepartmentsViewModels;

namespace RazorPageApp.Pages.Department
{
    public class UpdateDepartmentModel : PageModel
    {
        private readonly IDepartmentService _departmentService;

        public UpdateDepartmentModel(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [BindProperty]
        public UpdateDepartmentViewModel UpdateDepartmentViewModel { get; set; }

        public string ErrorMessage { get; private set; }

        public async Task<IActionResult> OnGetAsync(int departmentId)
        {
            try
            {
                DepartmentDetailsViewModel departmentDetailsViewModel = await _departmentService.GetDepartmentAsync(departmentId);

                if (departmentDetailsViewModel == null)
                {
                    return NotFound();
                }

                UpdateDepartmentViewModel = new UpdateDepartmentViewModel()
                {
                    DepartmentId = departmentDetailsViewModel.DepartmentId,
                    DepartmentName = departmentDetailsViewModel.DepartmentName,
                    Description = departmentDetailsViewModel.Description,
                };
            }
            catch (Exception)
            {
                ErrorMessage = "There is some problem with the service. Please try again. If the problem persists then contract with the system administrator.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _departmentService.UpdateDepartmentAsync(UpdateDepartmentViewModel);
                    return RedirectToPage("./DepartmentList");
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "There is some problem with the service. Please try again. If the problem persists then contract with the system administrator.");
                }
            }

            return Page();
        }
    }
}