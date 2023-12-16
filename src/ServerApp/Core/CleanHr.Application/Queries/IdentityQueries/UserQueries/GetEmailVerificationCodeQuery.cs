using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Queries.IdentityQueries.UserQueries;

public sealed class GetEmailVerificationCodeQuery(string email, string code) : IRequest<EmailVerificationCode>
{
    public string Email { get; } = email.ThrowIfNotValidEmail(nameof(email));

    public string Code { get; } = code.ThrowIfNullOrEmpty(nameof(code));
}

internal class GetEmailVerificationCodeQueryHandler(
        IRepository repository) : IRequestHandler<GetEmailVerificationCodeQuery, EmailVerificationCode>
{
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<EmailVerificationCode> Handle(
        GetEmailVerificationCodeQuery request,
        CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        EmailVerificationCode emailVerificationCode = await _repository
        .GetAsync<EmailVerificationCode>(evc => evc.Email == request.Email && evc.Code == request.Code && evc.UsedAtUtc == null, cancellationToken);

        return emailVerificationCode;
    }
}
