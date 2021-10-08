using System;
using System.Threading;
using System.Threading.Tasks;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace Identity.Application.Commands.UserCommands
{
    public class StoreUserPasswordCommand : IRequest
    {
        public StoreUserPasswordCommand(User user, string password)
        {
            User = user.ThrowIfNull(nameof(user));
            Password = password.ThrowIfNullOrEmpty(nameof(password));
        }

        public User User { get; }

        public string Password { get; }

        private class StoreUserPasswordCommandHandler : IRequestHandler<StoreUserPasswordCommand>
        {
            private readonly IPasswordHasher<User> _passwordHasher;
            private readonly IRepository _repository;

            public StoreUserPasswordCommandHandler(IPasswordHasher<User> passwordHasher, IRepository repository)
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

                await _repository.InsertAsync(userOldPassword);

                return Unit.Value;
            }
        }
    }
}
