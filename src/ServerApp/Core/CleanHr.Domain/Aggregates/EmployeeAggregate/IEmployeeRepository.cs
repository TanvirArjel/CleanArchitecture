using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CleanHr.Domain.Aggregates.EmployeeAggregate;

public interface IEmployeeRepository
{
    Task<Employee> GetByIdAsync(Guid employeeId);

    Task<bool> ExistsAsync(Expression<Func<Employee, bool>> condition, CancellationToken cancellationToken = default);

    Task InsertAsync(Employee employee);

    Task UpdateAsync(Employee employeeToBeUpdated);

    Task DeleteAsync(Employee employeeToBeDeleted);
}
