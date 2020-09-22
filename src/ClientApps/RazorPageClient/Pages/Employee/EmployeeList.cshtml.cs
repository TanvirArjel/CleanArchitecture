using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPageClient.Services;
using RazorPageClient.ViewModels.EmployeeViewModels;

namespace RazorPageClient.Pages.Employee
{
    public class EmployeeListModel : PageModel
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeListModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public List<EmployeeDetailsViewModel> EmployeeList { get; private set; }

        public string ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                EmployeeList = await _employeeService.GetEmployeeListAsync();
            }
            catch (Exception)
            {
                ErrorMessage = "There is some problem with the service. Please try again. If the problem persist, Please contract with system administrator.";
            }
        }
    }
}