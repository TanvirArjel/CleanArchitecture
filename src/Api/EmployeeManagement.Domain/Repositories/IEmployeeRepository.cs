using System.Linq;
using System.Threading.Tasks;
using AspNetCore.ServiceRegistration.Dynamic;
using EmployeeManagement.Domain.Dtos;
using EmployeeManagement.Domain.Dtos.EmployeeDtos;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Domain.Repositories
{
    public interface IEmployeeRepository : IScopedService
    {
        IQueryable<Employee> Employees { get; }

        Task<PaginatedList<EmployeeDetailsDto>> GetListAsync(int pageNumber, int pageSize);

        Task<Employee> GetByIdAsync(long employeeId);

        Task InsertAsync(Employee employee);

        Task UpdateAsync(Employee employee);

        Task DeleteAsync(Employee employee);
    }
}
