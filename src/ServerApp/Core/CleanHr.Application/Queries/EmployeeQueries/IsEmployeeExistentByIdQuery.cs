using CleanHr.Domain.Aggregates.EmployeeAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Application.Queries.EmployeeQueries;

public sealed class IsEmployeeExistentByIdQuery(Guid employeeId) : IRequest<bool>
{
    public Guid Id { get; } = employeeId.ThrowIfEmpty(nameof(employeeId));
}

internal class IsEmployeeExistentByIdQueryHandler(IEmployeeRepository employeeRepository) : IRequestHandler<IsEmployeeExistentByIdQuery, bool>
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));

    public async Task<bool> Handle(IsEmployeeExistentByIdQuery request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        bool isExistent = await _employeeRepository.ExistsAsync(e => e.Id == request.Id);
        return isExistent;
    }
}
