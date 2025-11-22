using System;
using System.Text.RegularExpressions;
using FluentValidation;

namespace CleanHr.Domain.Aggregates.EmployeeAggregate.Validators;

public class EmployeeValidator : AbstractValidator<Employee>
{
    public EmployeeValidator()
    {
        // FirstName validation
        RuleFor(e => e.FirstName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("The {PropertyName} is required.")
            .MinimumLength(2)
            .WithMessage("The {PropertyName} must be at least {MinLength} characters.")
            .MaximumLength(50)
            .WithMessage("The {PropertyName} can't be more than {MaxLength} characters.");

        // LastName validation
        RuleFor(e => e.LastName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("The {PropertyName} is required.")
            .MinimumLength(2)
            .WithMessage("The {PropertyName} must be at least {MinLength} characters.")
            .MaximumLength(50)
            .WithMessage("The {PropertyName} can't be more than {MaxLength} characters.");

        // DepartmentId validation
        RuleFor(e => e.DepartmentId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("The {PropertyName} is required.");

        // DateOfBirth validation
        RuleFor(e => e.DateOfBirth)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("The {PropertyName} is required.")
            .Must(BeValidDateOfBirth)
            .WithMessage("The employee must be at least 15 years old and not more than 115 years old.")
            .LessThanOrEqualTo(DateTime.Today)
            .WithMessage("The {PropertyName} cannot be in the future.");

        // Email validation
        RuleFor(e => e.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("The {PropertyName} is required.")
            .MaximumLength(50)
            .WithMessage("The {PropertyName} can't be more than {MaxLength} characters.")
            .Must(BeValidEmail)
            .WithMessage("The {PropertyName} is not a valid email address.");

        // PhoneNumber validation
        RuleFor(e => e.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("The {PropertyName} is required.")
            .MinimumLength(10)
            .WithMessage("The {PropertyName} must be at least {MinLength} characters.")
            .MaximumLength(20)
            .WithMessage("The {PropertyName} can't be more than {MaxLength} characters.");
    }

    private bool BeValidDateOfBirth(DateTime dateOfBirth)
    {
        DateTime minDateOfBirth = DateTime.UtcNow.AddYears(-115);
        DateTime maxDateOfBirth = DateTime.UtcNow.AddYears(-15);

        return dateOfBirth >= minDateOfBirth && dateOfBirth <= maxDateOfBirth;
    }

    private bool BeValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        Regex emailRegex = new(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        return emailRegex.IsMatch(email);
    }
}
