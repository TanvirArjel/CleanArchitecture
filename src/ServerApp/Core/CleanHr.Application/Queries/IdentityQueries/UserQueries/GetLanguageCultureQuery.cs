using System.Linq;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Queries.IdentityQueries.UserQueries;

public sealed class GetLanguageCultureQuery(Guid userId) : IRequest<string>
{
    public Guid UserId { get; } = userId.ThrowIfEmpty(nameof(userId));
}

internal class GetLanguageCultureQueryHandler(IRepository repository) : IRequestHandler<GetLanguageCultureQuery, string>
{
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<string> Handle(GetLanguageCultureQuery request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        string userLanguageCulture = await _repository.GetQueryable<ApplicationUser>().Where(u => u.Id == request.UserId)
            .Select(u => u.LanguageCulture).FirstOrDefaultAsync(cancellationToken);

        return userLanguageCulture;
    }
}
