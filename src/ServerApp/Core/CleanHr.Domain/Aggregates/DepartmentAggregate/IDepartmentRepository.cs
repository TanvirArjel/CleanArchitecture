using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CleanHr.Domain.Aggregates.DepartmentAggregate;

public interface IDepartmentRepository
{
    Task<Department> GetByIdAsync(Guid departmentId);

    Task<bool> ExistsAsync(Expression<Func<Department, bool>> condition, CancellationToken cancellationToken = default);

    Task InsertAsync(Department department);

    Task UpdateAsync(Department department);

    Task DeleteAsync(Department department);
}
