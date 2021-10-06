using System;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Aggregates.DepartmentAggregate
{
    public interface IDepartmentRepository
    {
        Task<Department> GetByIdAsync(Guid departmentId);

        Task InsertAsync(Department department);

        Task UpdateAsync(Department department);

        Task DeleteAsync(Department department);
    }
}
