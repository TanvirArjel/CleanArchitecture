using CleanHr.Domain.Aggregates.EmployeeAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Application.Queries.EmployeeQueries;

public sealed class IsEmployeeExistentByIdQuery(Guid employeeId) : IRequest<bool>
{

    public Guid Id { get; } = employeeId.ThrowIfEmpty(nameof(employeeId));

    private class IsEmployeeExistentByIdQueryHandler(IEmployeeRepository employeeRepository) : IRequestHandler<IsEmployeeExistentByIdQuery, bool>
    {

        public async Task<bool> Handle(IsEmployeeExistentByIdQuery request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            bool isExistent = await employeeRepository.ExistsAsync(e => e.Id == request.Id);
            return isExistent;
        }
    }
}
