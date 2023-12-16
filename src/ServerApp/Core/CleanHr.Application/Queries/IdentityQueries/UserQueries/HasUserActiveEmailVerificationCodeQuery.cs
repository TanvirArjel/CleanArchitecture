using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Queries.IdentityQueries.UserQueries;

public sealed class HasUserActiveEmailVerificationCodeQuery(string email) : IRequest<bool>
{
    public string Email { get; } = email.ThrowIfNotValidEmail(nameof(email));

    private class HasUserActiveEmailVerificationCodeQueryHandler(IRepository repository) : IRequestHandler<HasUserActiveEmailVerificationCodeQuery, bool>
    {
        public async Task<bool> Handle(HasUserActiveEmailVerificationCodeQuery request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            bool isExists = await repository
            .ExistsAsync<EmailVerificationCode>(evc => evc.Email == request.Email && evc.SentAtUtc.AddMinutes(5) > DateTime.UtcNow, cancellationToken);
            return isExists;
        }
    }
}
