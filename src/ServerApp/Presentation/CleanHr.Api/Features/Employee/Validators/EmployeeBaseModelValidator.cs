using FluentValidation;

namespace CleanHr.Api.Features.Employee.Validators;

public class EmployeeBaseModelValidator<T> : AbstractValidator<T>
    where T : EmployeeBaseModel
{
    public EmployeeBaseModelValidator()
    {
        RuleFor(e => e.Name).NotEmpty()
                            .MinimumLength(5).WithMessage("The Name must be at least 5 characters.")
                            .MaximumLength(50).WithMessage("The Name can't be more than 50 characters.");

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