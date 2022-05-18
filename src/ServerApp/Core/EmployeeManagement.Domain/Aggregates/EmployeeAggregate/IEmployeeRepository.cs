using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain.Aggregates.EmployeeAggregate;

public interface IEmployeeRepository
{
    Task<Employee> GetByIdAsync(Guid employeeId);

    Task<bool> ExistsAsync(Expression<Func<Employee, bool>> condition);

    Task InsertAsync(Employee employee);

    Task UpdateAsync(Employee employeeToBeUpdated);

    Task DeleteAsync(Employee employeeToBeDeleted);
}
