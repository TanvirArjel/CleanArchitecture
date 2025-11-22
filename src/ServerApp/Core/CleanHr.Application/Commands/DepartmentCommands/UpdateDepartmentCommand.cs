using CleanHr.Application.Caching.Handlers;
using CleanHr.Domain;
using CleanHr.Domain.Aggregates.DepartmentAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Application.Commands.DepartmentCommands;

public sealed record UpdateDepartmentCommand(
    Guid Id,
    string Name,
    string Description,
    bool IsActive) : IRequest<Result>;

internal sealed class UpdateDepartmentCommandHandler(
    IDepartmentRepository departmentRepository,
    IDepartmentCacheHandler departmentCacheHandler) : IRequestHandler<UpdateDepartmentCommand, Result>
{
    public async Task<Result> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        Department departmentToBeUpdated = await departmentRepository.GetByIdAsync(request.Id);

        if (departmentToBeUpdated == null)
        {
            return Result.Failure("DepartmentId", $"The department with id '{request.Id}' was not found.");
        }

        Result setNameResult = await departmentToBeUpdated.SetNameAsync(departmentRepository, request.Name);
        if (setNameResult.IsSuccess == false)
        {
            return setNameResult;
        }

        Result setDescriptionResult = departmentToBeUpdated.SetDescription(request.Description);
        if (setDescriptionResult.IsSuccess == false)
        {
            return setDescriptionResult;
        }

        departmentToBeUpdated.IsActive = request.IsActive;

        await departmentRepository.UpdateAsync(departmentToBeUpdated);

        await departmentCacheHandler.RemoveListAsync();
        return Result.Success();
    }
}
