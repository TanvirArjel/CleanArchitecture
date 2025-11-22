using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CleanHr.Domain.Aggregates.EmployeeAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Persistence.RelationalDB.Repositories;

internal sealed class EmployeeRepository(CleanHrDbContext dbContext) : IEmployeeRepository
{
    public async Task<Employee> GetByIdAsync(Guid employeeId)
    {
        employeeId.ThrowIfEmpty(nameof(employeeId));

        Employee employee = await dbContext.Set<Employee>().FindAsync(employeeId);
        return employee;
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<Employee, bool>> condition,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Employee> queryable = dbContext.Set<Employee>();

        if (condition != null)
        {
            queryable = queryable.Where(condition);
        }

        return await queryable.AnyAsync(cancellationToken);
    }

    public async Task InsertAsync(Employee employee)
    {
        employee.ThrowIfNull(nameof(employee));

        await dbContext.Set<Employee>().AddAsync(employee);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Employee employeeToBeUpdated)
    {
        employeeToBeUpdated.ThrowIfNull(nameof(employeeToBeUpdated));

        EntityEntry<Employee> trackedEntity = dbContext.ChangeTracker.Entries<Employee>()
           .FirstOrDefault(x => x.Entity == employeeToBeUpdated);

        if (trackedEntity == null)
        {
            dbContext.Update(employeeToBeUpdated);
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Employee employeeToBeDeleted)
    {
        employeeToBeDeleted.ThrowIfNull(nameof(employeeToBeDeleted));

        dbContext.Set<Employee>().Remove(employeeToBeDeleted);
        await dbContext.SaveChangesAsync();
    }
}
