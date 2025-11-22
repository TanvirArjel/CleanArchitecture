using System.Data;
using CleanHr.Domain;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Commands.IdentityCommands.UserCommands;

public sealed class VerifyUserEmailCommand(string email, string code) : IRequest<Result>
{
    public string Email { get; } = email.ThrowIfNotValidEmail(nameof(email));

    public string Code { get; } = code.ThrowIfNullOrEmpty(nameof(code));
}

internal class VerifyUserEmailCommandHandler(IRepository repository) : IRequestHandler<VerifyUserEmailCommand, Result>
{
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<Result> Handle(VerifyUserEmailCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        IDbContextTransaction dbContextTransaction = await _repository
            .BeginTransactionAsync(IsolationLevel.Unspecified, cancellationToken);

        try
        {
            EmailVerificationCode emailVerificationCode = await _repository
            .GetAsync<EmailVerificationCode>(evc => evc.Email == request.Email && evc.Code == request.Code && evc.UsedAtUtc == null, cancellationToken);

            if (emailVerificationCode == null)
            {
                await dbContextTransaction.RollbackAsync(cancellationToken);
                return Result.Failure("Either email or password reset code is incorrect.");
            }

            if (DateTime.UtcNow > emailVerificationCode.SentAtUtc.AddMinutes(5))
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

            applicationUser.EmailConfirmed = true;
            _repository.Update(applicationUser);

            emailVerificationCode.MarkAsUsed();
            _repository.Update(emailVerificationCode);

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
