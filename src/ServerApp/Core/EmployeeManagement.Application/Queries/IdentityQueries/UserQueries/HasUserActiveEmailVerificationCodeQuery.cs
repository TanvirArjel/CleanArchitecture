using EmployeeManagement.Domain.Aggregates.IdentityAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace EmployeeManagement.Application.Queries.IdentityQueries.UserQueries;

public sealed class HasUserActiveEmailVerificationCodeQuery : IRequest<bool>
{
    public HasUserActiveEmailVerificationCodeQuery(string email)
    {
        Email = email.ThrowIfNotValidEmail(nameof(email));
    }

    public string Email { get; }

    private class HasUserActiveEmailVerificationCodeQueryHandler : IRequestHandler<HasUserActiveEmailVerificationCodeQuery, bool>
    {
        private readonly IRepository _repository;

        public HasUserActiveEmailVerificationCodeQueryHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(HasUserActiveEmailVerificationCodeQuery request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            bool isExists = await _repository
            .ExistsAsync<EmailVerificationCode>(evc => evc.Email == request.Email && evc.SentAtUtc.AddMinutes(5) > DateTime.UtcNow, cancellationToken);
            return isExists;
        }
    }
}
