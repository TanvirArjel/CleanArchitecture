using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.ServiceRegistration.Dynamic;
using RazorPageClient.ViewModels.EmployeeViewModels;

namespace RazorPageClient.Services
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
