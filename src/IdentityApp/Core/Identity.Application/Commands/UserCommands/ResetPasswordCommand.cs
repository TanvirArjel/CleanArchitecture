using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace Identity.Application.Commands.UserCommands;

public class ResetPasswordCommand : IRequest
{
    public ResetPasswordCommand(string email, string code, string newPassword)
    {
        Email = email.ThrowIfNotValidEmail(nameof(email));
        Code = code.ThrowIfNullOrEmpty(nameof(code));
        NewPassword = newPassword.ThrowIfNullOrEmpty(nameof(newPassword));
    }

    public string Email { get; }

    public string Code { get; }

    public string NewPassword { get; }

    private class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
    {
        private readonly IRepository _repository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public ResetPasswordCommandHandler(IRepository repository, IPasswordHasher<User> passwordHasher)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            IDbContextTransaction dbContextTransaction = await _repository
                .BeginTransactionAsync(IsolationLevel.Unspecified, cancellationToken);

            try
            {
                PasswordResetCode passwordResetCode = await _repository
                .GetAsync<PasswordResetCode>(evc => evc.Email == request.Email && evc.Code == request.Code && evc.UsedAtUtc == null, cancellationToken);

                if (passwordResetCode == null)
                {
                    throw new InvalidOperationException("Either email or password reset code is incorrect.");
                }

                if (DateTime.UtcNow > passwordResetCode.SentAtUtc.AddMinutes(5))
                {
                    throw new InvalidOperationException("The code is expired.");
                }

                User applicationUser = await _repository.GetAsync<User>(au => au.Email == request.Email, cancellationToken);

                if (applicationUser == null)
                {
                    throw new InvalidOperationException("The provided email is not related to any account.");
                }

                string newHashedPassword = _passwordHasher.HashPassword(applicationUser, request.NewPassword);
                applicationUser.PasswordHash = newHashedPassword;
                await _repository.UpdateAsync(applicationUser, cancellationToken);

                passwordResetCode.UsedAtUtc = DateTime.UtcNow;
                await _repository.UpdateAsync(passwordResetCode, cancellationToken);

                await dbContextTransaction.CommitAsync(cancellationToken);

                return Unit.Value;
            }
            catch (Exception)
            {
                await dbContextTransaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
