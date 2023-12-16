using System.Threading;
using CleanHr.Api.Features.Department.Models;
using CleanHr.Application.Queries.DepartmentQueries;
using FluentValidation;
using MediatR;

namespace CleanHr.Api.Features.Department.Validators;

public abstract class DepartmentBaseModelValidator<T> : AbstractValidator<T>
    where T : DepartmentBaseModel
{
    private readonly IMediator _mediator;

    protected DepartmentBaseModelValidator(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        RuleFor(d => d.Description)
               .NotEmpty()
               .WithMessage("The {PropertyName} is required.")
               .MinimumLength(20)
               .WithMessage("The {PropertyName} must be at least {MinLength} characters.")
               .MaximumLength(200)
               .WithMessage("The {PropertyName} can't be more than {MaxLength} characters.");
    }

    protected async Task<bool> IsUniqueNameAsync(Guid id, string name, CancellationToken cancellationToken)
    {
        IsDepartmentNameUniqueQuery nameUniqueQuery = new(id, name);
        bool isUnique = await _mediator.Send(nameUniqueQuery, cancellationToken);
        return isUnique;
    }
}
