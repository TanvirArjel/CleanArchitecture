using FluentValidation;

namespace CleanHr.Api.Features.Employee.Validators;

internal abstract class EmployeeBaseModelValidator<T> : AbstractValidator<T>
    where T : EmployeeBaseModel
{
    protected EmployeeBaseModelValidator()
    {
        RuleFor(e => e.FirstName).NotEmpty()
                            .MinimumLength(2).WithMessage("The FirstName must be at least 2 characters.")
                            .MaximumLength(50).WithMessage("The FirstName can't be more than 50 characters.");

        RuleFor(e => e.LastName).NotEmpty()
                            .MinimumLength(2).WithMessage("The LastName must be at least 2 characters.")
                            .MaximumLength(50).WithMessage("The LastName can't be more than 50 characters.");

        RuleFor(e => e.DepartmentId).NotEmpty();

        RuleFor(e => e.DateOfBirth).Must(IsAdult).WithMessage("The minimum age has to be 15 years.");

        RuleFor(e => e.Email).NotEmpty()
                             .EmailAddress()
                             .MinimumLength(8).WithMessage("The Email must be at least 5 characters.")
                             .MaximumLength(50).WithMessage("The Email can't be more than 50 characters.");

        RuleFor(e => e.PhoneNumber).NotEmpty()
                                   .MinimumLength(8).WithMessage("The PhoneNumber must be at least 5 characters.")
                                   .MaximumLength(50).WithMessage("The PhoneNumber can't be more than 50 characters.");
    }

    private bool IsAdult(DateTime dateOfBirth)
    {
        return true;
    }
}