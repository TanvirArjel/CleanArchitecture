using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CleanHr.Domain.Aggregates.DepartmentAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Persistence.RelationalDB.Repositories;

internal sealed class DepartmentRepository(CleanHrDbContext dbContext) : IDepartmentRepository
{
    public Task<bool> ExistsAsync(Expression<Func<Department, bool>> condition, CancellationToken cancellationToken = default)
    {
        IQueryable<Department> queryable = dbContext.Set<Department>();

        if (condition != null)
        {
            queryable = queryable.Where(condition);
        }

        return queryable.AnyAsync(cancellationToken);
    }

    public async Task<Department> GetByIdAsync(Guid departmentId)
    {
        departmentId.ThrowIfEmpty(nameof(departmentId));

        Department department = await dbContext.Set<Department>().FindAsync(departmentId);
        return department;
    }

    public async Task InsertAsync(Department department)
    {
        department.ThrowIfNull(nameof(department));

        await dbContext.AddAsync(department);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Department department)
    {
        department.ThrowIfNull(nameof(department));

        EntityEntry<Department> trackedEntity = dbContext.ChangeTracker.Entries<Department>()
            .FirstOrDefault(x => x.Entity == department);

        if (trackedEntity == null)
        {
            dbContext.Update(department);
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Department department)
    {
        department.ThrowIfNull(nameof(department));

        dbContext.Remove(department);
        await dbContext.SaveChangesAsync();
    }
}
