using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Queries.IdentityQueries.UserQueries;

public sealed class GetEmailVerificationCodeQuery : IRequest<EmailVerificationCode>
{
    public GetEmailVerificationCodeQuery(string email, string code)
    {
        Email = email.ThrowIfNotValidEmail(nameof(email));
        Code = code.ThrowIfNullOrEmpty(nameof(code));
    }

    public string Email { get; }

    public string Code { get; }

    private class GetEmailVerificationCodeQueryHandler : IRequestHandler<GetEmailVerificationCodeQuery, EmailVerificationCode>
    {
        private readonly IRepository _repository;

        public GetEmailVerificationCodeQueryHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<EmailVerificationCode> Handle(GetEmailVerificationCodeQuery request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            EmailVerificationCode emailVerificationCode = await _repository
            .GetAsync<EmailVerificationCode>(evc => evc.Email == request.Email && evc.Code == request.Code && evc.UsedAtUtc == null, cancellationToken);

            return emailVerificationCode;
        }
    }
}
