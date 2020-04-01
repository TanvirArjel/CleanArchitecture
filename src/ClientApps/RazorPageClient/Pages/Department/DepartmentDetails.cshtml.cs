// <copyright file="DepartmentDetails.cshtml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPageClient.Services.DepartmentService;
using RazorPageClient.ViewModels.DepartmentsViewModels;
using System.Threading.Tasks;

namespace RazorPageClient.Pages.Department
{
    public class DepartmentDetailsModel : PageModel
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentDetailsModel(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public DepartmentDetailsViewModel DepartmentDetailsViewModel { get; private set; }

        public async Task<IActionResult> OnGetAsync(int departmentId)
        {
            DepartmentDetailsViewModel departmentDetailsViewModel = await _departmentService.GetDepartmentAsync(departmentId);
            if (departmentDetailsViewModel == null)
            {
                return NotFound();
            }

            DepartmentDetailsViewModel = departmentDetailsViewModel;
            return Page();
        }
    }
}