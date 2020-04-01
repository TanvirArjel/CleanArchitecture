using EmployeeManagement.Application.Dtos.EmployeeDtos;
using EmployeeManagement.Common.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Services.EmployeeService
{
    public interface IEmployeeService : IScopedService
    {
        Task<List<EmployeeDetailsDto>> GetEmployeeListAsync();

        Task<EmployeeDetailsDto> GetEmployeeDetailsAsync(int employeeId);

        Task CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto);

        Task UpdateEmplyeeAsync(UpdateEmployeeDto updateEmployeeDto);

        Task DeleteEmployee(int employeeId);
    }
}
