using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CleanHr.Domain.Aggregates.DepartmentAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Persistence.RelationalDB.Repositories;

internal sealed class DepartmentRepository : IDepartmentRepository
{
    private readonly CleanHrDbContext _dbContext;

    public DepartmentRepository(CleanHrDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsAsync(Expression<Func<Department, bool>> condition)
    {
        IQueryable<Department> queryable = _dbContext.Set<Department>();

        if (condition != null)
        {
            queryable = queryable.Where(condition);
        }

        return queryable.AnyAsync();
    }

    public async Task<Department> GetByIdAsync(Guid departmentId)
    {
        departmentId.ThrowIfEmpty(nameof(departmentId));

        Department department = await _dbContext.Set<Department>().FindAsync(departmentId);
        return department;
    }

    public async Task InsertAsync(Department department)
    {
        department.ThrowIfNull(nameof(department));

        await _dbContext.AddAsync(department);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Department department)
    {
        department.ThrowIfNull(nameof(department));

        EntityEntry<Department> trackedEntity = _dbContext.ChangeTracker.Entries<Department>()
            .FirstOrDefault(x => x.Entity == department);

        if (trackedEntity == null)
        {
            _dbContext.Update(department);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Department department)
    {
        department.ThrowIfNull(nameof(department));

        _dbContext.Remove(department);
        await _dbContext.SaveChangesAsync();
    }
}
