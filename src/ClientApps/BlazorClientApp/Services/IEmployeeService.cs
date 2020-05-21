using AspNetCore.ServiceRegistration.Dynamic.Interfaces;
using BlazorClientApp.ViewModels.EmployeeViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorClientApp.Services
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
