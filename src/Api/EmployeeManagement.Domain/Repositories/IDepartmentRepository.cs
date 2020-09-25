using System.Linq;
using System.Threading.Tasks;
using AspNetCore.ServiceRegistration.Dynamic;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Domain.Repositories
{
    public interface IDepartmentRepository : IScopedService
    {
        IQueryable<Department> Departments { get; }

        Task<Department> GetByIdAsync(long departmentId);

        Task<int> InsertAsync(Department department);

        Task UpdateAsync(Department department);

        Task DeleteAsync(Department department);
    }
}
