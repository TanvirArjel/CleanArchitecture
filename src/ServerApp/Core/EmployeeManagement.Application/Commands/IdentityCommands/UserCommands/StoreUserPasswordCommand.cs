using System;
using System.Threading;
using System.Threading.Tasks;
using EmployeeManagement.Domain.Aggregates.IdentityAggregate;
using MediatR;
using Microsoft.AspNetCore.Identity;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace EmployeeManagement.Application.Commands.IdentityCommands.UserCommands;

public class StoreUserPasswordCommand : IRequest
{
    public StoreUserPasswordCommand(ApplicationUser user, string password)
    {
        User = user.ThrowIfNull(nameof(user));
        Password = password.ThrowIfNullOrEmpty(nameof(password));
    }

    public ApplicationUser User { get; }

    public string Password { get; }

    private class StoreUserPasswordCommandHandler : IRequestHandler<StoreUserPasswordCommand>
    {
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly IRepository _repository;

        public StoreUserPasswordCommandHandler(IPasswordHasher<ApplicationUser> passwordHasher, IRepository repository)
        {
            _passwordHasher = passwordHasher;
            _repository = repository;
        }

        public async Task<Unit> Handle(StoreUserPasswordCommand request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            string passwordHash = _passwordHasher.HashPassword(request.User, request.Password);

            UserOldPassword userOldPassword = new UserOldPassword()
            {
                UserId = request.User.Id,
                PasswordHash = passwordHash,
                SetAtUtc = DateTime.UtcNow
            };

            await _repository.InsertAsync(userOldPassword, cancellationToken);

            return Unit.Value;
        }
    }
}
