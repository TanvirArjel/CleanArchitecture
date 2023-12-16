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
}

internal class CheckIfOldPasswordQueryHandler(
    IRepository repository,
    IPasswordHasher<ApplicationUser> passwordHasher) : IRequestHandler<CheckIfOldPasswordQuery, bool>
{
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));

    public async Task<bool> Handle(CheckIfOldPasswordQuery request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        List<UserOldPassword> userOldPasswords = await _repository.GetQueryable<UserOldPassword>()
            .Where(uop => uop.UserId == request.User.Id).ToListAsync(cancellationToken);

        if (userOldPasswords.Count == 0)
        {
            return false;
        }

        foreach (UserOldPassword item in userOldPasswords)
        {
            PasswordVerificationResult passwordVerificationResult = _passwordHasher.VerifyHashedPassword(request.User, item.PasswordHash, request.Password);
            if (passwordVerificationResult == PasswordVerificationResult.Success)
            {
                return true;
            }
        }

        return false;
    }
}
