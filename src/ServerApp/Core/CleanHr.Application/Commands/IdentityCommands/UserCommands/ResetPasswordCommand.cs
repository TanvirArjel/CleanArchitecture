using System.Data;
using CleanHr.Domain;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Commands.IdentityCommands.UserCommands;

public sealed record ResetPasswordCommand(string Email, string Code, string NewPassword) : IRequest<Result>;

internal class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IRepository _repository;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;

    public ResetPasswordCommandHandler(
        IRepository repository,
        IPasswordHasher<ApplicationUser> passwordHasher)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
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
                await dbContextTransaction.RollbackAsync(cancellationToken);
                return Result.Failure("Either email or password reset code is incorrect.");
            }

            if (DateTime.UtcNow > passwordResetCode.SentAtUtc.AddMinutes(5))
            {
                await dbContextTransaction.RollbackAsync(cancellationToken);
                return Result.Failure("The code is expired.");
            }

            ApplicationUser applicationUser = await _repository.GetAsync<ApplicationUser>(au => au.Email == request.Email, cancellationToken);

            if (applicationUser == null)
            {
                await dbContextTransaction.RollbackAsync(cancellationToken);
                return Result.Failure("The provided email is not related to any account.");
            }

            // Use domain method to set password (includes validation)
            Result setPasswordResult = await applicationUser.SetPasswordAsync(request.NewPassword, _passwordHasher);

            if (setPasswordResult.IsSuccess == false)
            {
                await dbContextTransaction.RollbackAsync(cancellationToken);
                return setPasswordResult;
            }

            _repository.Update(applicationUser);

            passwordResetCode.MarkAsUsed();
            _repository.Update(passwordResetCode);

            await dbContextTransaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception)
        {
            await dbContextTransaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
