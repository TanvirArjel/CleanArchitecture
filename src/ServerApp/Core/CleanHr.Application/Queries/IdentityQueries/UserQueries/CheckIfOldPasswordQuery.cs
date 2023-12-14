using System.Linq;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Queries.IdentityQueries.UserQueries;

public sealed class CheckIfOldPasswordQuery(ApplicationUser user, string password) : IRequest<bool>
{
    public ApplicationUser User { get; } = user.ThrowIfNull(nameof(user));

    public string Password { get; } = password.ThrowIfNullOrEmpty(nameof(password));

    private class CheckIfOldPasswordQueryHandler(IRepository repository, IPasswordHasher<ApplicationUser> passwordHasher) : IRequestHandler<CheckIfOldPasswordQuery, bool>
    {

        public async Task<bool> Handle(CheckIfOldPasswordQuery request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            List<UserOldPassword> userOldPasswords = await repository.GetQueryable<UserOldPassword>()
                .Where(uop => uop.UserId == request.User.Id).ToListAsync(cancellationToken);

            if (userOldPasswords.Count == 0)
            {
                return false;
            }

            foreach (UserOldPassword item in userOldPasswords)
            {
                PasswordVerificationResult passwordVerificationResult = passwordHasher.VerifyHashedPassword(request.User, item.PasswordHash, request.Password);
                if (passwordVerificationResult == PasswordVerificationResult.Success)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
