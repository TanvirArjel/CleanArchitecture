using System.Data;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Commands.IdentityCommands.UserCommands;

public sealed record ResetPasswordCommand(string Email, string Code, string NewPassword) : IRequest;

internal class ResetPasswordCommandHandler(
    IRepository repository,
    IPasswordHasher<ApplicationUser> passwordHasher) : IRequestHandler<ResetPasswordCommand>
{
    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        IDbContextTransaction dbContextTransaction = await repository
                .BeginTransactionAsync(IsolationLevel.Unspecified, cancellationToken);

        try
        {
            PasswordResetCode passwordResetCode = await repository
                .GetAsync<PasswordResetCode>(evc => evc.Email == request.Email && evc.Code == request.Code && evc.UsedAtUtc == null, cancellationToken);

            if (passwordResetCode == null)
            {
                throw new InvalidOperationException("Either email or password reset code is incorrect.");
            }

            if (DateTime.UtcNow > passwordResetCode.SentAtUtc.AddMinutes(5))
            {
                throw new InvalidOperationException("The code is expired.");
            }

            ApplicationUser applicationUser = await repository.GetAsync<ApplicationUser>(au => au.Email == request.Email, cancellationToken);

            if (applicationUser == null)
            {
                throw new InvalidOperationException("The provided email is not related to any account.");
            }

            string newHashedPassword = passwordHasher.HashPassword(applicationUser, request.NewPassword);
            applicationUser.PasswordHash = newHashedPassword;
            repository.Update(applicationUser);

            passwordResetCode.UsedAtUtc = DateTime.UtcNow;
            repository.Update(passwordResetCode);

            await dbContextTransaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await dbContextTransaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
