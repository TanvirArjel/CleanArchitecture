using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Domain.Dtos.EmployeeDtos;
using EmployeeManagement.Domain.Entities;
using TanvirArjel.EFCore.GenericRepository;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Domain.Repositories
{
    [ScopedService]
    public interface IEmployeeRepository
    {
        IQueryable<Employee> Employees { get; }

        Task<PaginatedList<EmployeeDetailsDto>> GetListAsync(int pageNumber, int pageSize);

        Task<Employee> GetByIdAsync(long employeeId);

        Task<EmployeeDetailsDto> GetDetailsByIdAsync(long employeeId);

        Task InsertAsync(Employee employee);

        Task UpdateAsync(Employee employee);

        Task DeleteAsync(Employee employee);
    }
}
