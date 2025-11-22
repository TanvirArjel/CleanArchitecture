using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CleanHr.Domain.Aggregates.IdentityAggregate;

public interface IApplicationUserRepository
{
    Task<bool> ExistsAsync(Expression<Func<ApplicationUser, bool>> predicate);
    Task<ApplicationUser> GetByIdAsync(Guid id);
    Task<ApplicationUser> GetByEmailAsync(string email);
    Task<ApplicationUser> GetByUserNameAsync(string userName);
}
