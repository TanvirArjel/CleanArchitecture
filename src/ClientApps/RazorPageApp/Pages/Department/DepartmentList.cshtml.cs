﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPageApp.Services;
using RazorPageApp.ViewModels.DepartmentsViewModels;

namespace RazorPageApp.Pages.Department
{
    public class DepartmentListModel : PageModel
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentListModel(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public List<DepartmentDetailsViewModel> DepartmentList { get; private set; }

        public string ErrorMessage { get; private set; }

        public async Task OnGetAsync()
        {
            try
            {
                DepartmentList = await _departmentService.GetDepartmentListAsync();
            }
            catch (Exception)
            {
                ErrorMessage = "There is some problem with the service. Please try again. If the problem persist, Please contract with system administrator.";
            }
        }
    }
}