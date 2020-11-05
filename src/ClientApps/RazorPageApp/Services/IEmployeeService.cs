using System.Collections.Generic;
using System.Threading.Tasks;
using RazorPageApp.ViewModels.EmployeeViewModels;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace RazorPageApp.Services
{
    public interface IEmployeeService : IScopedService
    {
        Task<List<EmployeeDetailsViewModel>> GetEmployeeListAsync();

        Task<EmployeeDetailsViewModel> GetEmployeeDetailsAsync(int employeeId);

        Task CreateEmployeeAsync(CreateEmployeeViewModel createEmployeeViewModel);

        Task UpdateEmplyeeAsync(UpdateEmployeeViewModel updateEmployeeViewModel);

        Task DeleteEmployee(int employeeId);
    }
}
