using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Queries.IdentityQueries.UserQueries;

public sealed class GetPasswordResetCodeQuery(string email, string code) : IRequest<PasswordResetCode>
{
    public string Email { get; } = email.ThrowIfNotValidEmail(nameof(email));

    public string Code { get; } = code.ThrowIfNullOrEmpty(nameof(code));

    private class GetPasswordResetCodeQueryHandler(IRepository repository) : IRequestHandler<GetPasswordResetCodeQuery, PasswordResetCode>
    {
        public async Task<PasswordResetCode> Handle(GetPasswordResetCodeQuery request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            PasswordResetCode passwordResetCode = await repository
            .GetAsync<PasswordResetCode>(evc => evc.Email == request.Email && evc.Code == request.Code && evc.UsedAtUtc == null, cancellationToken);

            return passwordResetCode;
        }
    }
}
