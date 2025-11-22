using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using Microsoft.EntityFrameworkCore;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace CleanHr.Persistence.RelationalDB.Repositories;

[ScopedService]
internal sealed class ApplicationUserRepository : IApplicationUserRepository
{
    private readonly CleanHrDbContext _dbContext;

    public ApplicationUserRepository(CleanHrDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExistsAsync(Expression<Func<ApplicationUser, bool>> predicate)
    {
        return await _dbContext.Set<ApplicationUser>().AnyAsync(predicate);
    }

    public async Task<ApplicationUser> GetByIdAsync(Guid id)
    {
        return await _dbContext.Set<ApplicationUser>().FindAsync(id);
    }

    public async Task<ApplicationUser> GetByEmailAsync(string email)
    {
        return await _dbContext.Set<ApplicationUser>()
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<ApplicationUser> GetByUserNameAsync(string userName)
    {
        return await _dbContext.Set<ApplicationUser>()
            .FirstOrDefaultAsync(u => u.UserName == userName);
    }
}
