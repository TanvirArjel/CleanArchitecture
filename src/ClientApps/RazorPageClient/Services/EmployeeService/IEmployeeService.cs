using RazorPageClient.ViewModels.EmployeeViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPageClient.Services.EmployeeService
{
    public interface IEmployeeService
    {
        Task<List<EmployeeDetailsViewModel>> GetEmployeeListAsync();

        Task<EmployeeDetailsViewModel> GetEmployeeDetailsAsync(int employeeId);

        Task CreateEmployeeAsync(CreateEmployeeViewModel createEmployeeViewModel);

        Task UpdateEmplyeeAsync(UpdateEmployeeViewModel updateEmployeeViewModel);

        Task DeleteEmployee(int employeeId);
    }
}
