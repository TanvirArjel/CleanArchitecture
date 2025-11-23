using CleanHr.Application.Caching.Handlers;
using CleanHr.Domain;
using CleanHr.Domain.Aggregates.DepartmentAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Application.Commands.DepartmentCommands;

public sealed record CreateDepartmentCommand(string Name, string Description) : IRequest<Result<Guid>>;

internal class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Result<Guid>>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IDepartmentCacheHandler _departmentCacheHandler;

    public CreateDepartmentCommandHandler(
            IDepartmentRepository departmentRepository,
            IDepartmentCacheHandler departmentCacheHandler)
    {
        ArgumentNullException.ThrowIfNull(departmentCacheHandler);
        ArgumentNullException.ThrowIfNull(departmentCacheHandler);

        _departmentRepository = departmentRepository;
        _departmentCacheHandler = departmentCacheHandler;
    }

    public async Task<Result<Guid>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        _ = request.ThrowIfNull(nameof(request));


        Result<Department> result = await Department.CreateAsync(_departmentRepository, request.Name, request.Description);

        if (result.IsSuccess == false)
        {
            return Result<Guid>.Failure(result.Error);
        }

        // Persist to the database
        await _departmentRepository.InsertAsync(result.Value);

        // Remove the cache
        await _departmentCacheHandler.RemoveListAsync();

        return Result<Guid>.Success(result.Value.Id);
    }
}