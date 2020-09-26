using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.ServiceRegistration.Dynamic;
using EmployeeManagement.Application.Dtos.EmployeeDtos;
using EmployeeManagement.Domain.Dtos.EmployeeDtos;

namespace EmployeeManagement.Application.Services
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
