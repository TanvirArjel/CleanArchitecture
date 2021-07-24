using System;
using System.Threading.Tasks;
using EmployeeManagement.Application.Dtos.EmployeeDtos;
using TanvirArjel.EFCore.GenericRepository;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Application.Services
{
    [ScopedService]
    public interface IEmployeeService
    {
        Task<PaginatedList<EmployeeDetailsDto>> GetListAsync(int pageNumber, int pageSize);

        Task<EmployeeDetailsDto> GetDetailsByIdAsync(Guid employeeId);

        Task CreateAsync(CreateEmployeeDto createEmployeeDto);

        Task UpdateAsync(UpdateEmployeeDto updateEmployeeDto);

        Task DeleteAsync(Guid employeeId);
    }
}
