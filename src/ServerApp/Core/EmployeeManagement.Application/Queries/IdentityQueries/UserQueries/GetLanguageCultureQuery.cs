using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmployeeManagement.Domain.Aggregates.IdentityAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace EmployeeManagement.Application.Queries.IdentityQueries.UserQueries;

public class GetLanguageCultureQuery : IRequest<string>
{
    public GetLanguageCultureQuery(Guid userId)
    {
        UserId = userId.ThrowIfEmpty(nameof(userId));
    }

    public Guid UserId { get; }

    private class GetLanguageCultureQueryHandler : IRequestHandler<GetLanguageCultureQuery, string>
    {
        private readonly IRepository _repository;

        public GetLanguageCultureQueryHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<string> Handle(GetLanguageCultureQuery request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            string userLanguageCulture = await _repository.GetQueryable<User>().Where(u => u.Id == request.UserId)
                .Select(u => u.LanguageCulture).FirstOrDefaultAsync(cancellationToken);

            return userLanguageCulture;
        }
    }
}
