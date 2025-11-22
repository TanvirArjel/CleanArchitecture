using System;
using FluentValidation;

namespace CleanHr.Domain.Aggregates.EmployeeAggregate.Validators;

public class EmployeeDateOfBirthValidator : AbstractValidator<DateTime>
{
    public EmployeeDateOfBirthValidator()
    {
        RuleFor(dateOfBirth => dateOfBirth)
            .NotEmpty()
            .WithMessage("The DateOfBirth is required.")
            .Must(BeValidDateOfBirth)
            .WithMessage("The employee must be at least 15 years old and not more than 115 years old.")
            .LessThanOrEqualTo(DateTime.Today)
            .WithMessage("The DateOfBirth cannot be in the future.");
    }

    private bool BeValidDateOfBirth(DateTime dateOfBirth)
    {
        DateTime minDateOfBirth = DateTime.UtcNow.AddYears(-115);
        DateTime maxDateOfBirth = DateTime.UtcNow.AddYears(-15);

        return dateOfBirth >= minDateOfBirth && dateOfBirth <= maxDateOfBirth;
    }
}
