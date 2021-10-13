using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace Identity.Application.Queries.UserQueries
{
    public class CheckIfOldPasswordQuery : IRequest<bool>
    {
        public CheckIfOldPasswordQuery(User user, string password)
        {
            User = user.ThrowIfNull(nameof(user));
            Password = password.ThrowIfNullOrEmpty(nameof(password));
        }

        public User User { get; }

        public string Password { get; }

        private class CheckIfOldPasswordQueryHandler : IRequestHandler<CheckIfOldPasswordQuery, bool>
        {
            private readonly IRepository _repository;
            private readonly IPasswordHasher<User> _passwordHasher;

            public CheckIfOldPasswordQueryHandler(IRepository repository, IPasswordHasher<User> passwordHasher)
            {
                _repository = repository;
                _passwordHasher = passwordHasher;
            }

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
    }
}
