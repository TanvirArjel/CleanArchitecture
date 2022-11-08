using System.ComponentModel.DataAnnotations;
using CleanHr.Application.Commands.EmployeeCommands;
using CleanHr.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TanvirArjel.CustomValidation.Attributes;

namespace CleanHr.Api.Endpoints.Employees;

public class CreateEmployeeEndpoint : EmployeeEndpointBase
{
    private readonly IMediator _mediator;

    public CreateEmployeeEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST: api/employees
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Create a new employee by posting the required data.")]
    public async Task<ActionResult> Post([FromBody] CreateEmployeeModel model)
    {
        try
        {
            CreateEmployeeCommand createEmployeeCommand = new CreateEmployeeCommand(
                model.Name,
                model.Name,
                model.DepartmentId,
                model.DateOfBirth,
                model.Email,
                model.PhoneNumber);

            await _mediator.Send(createEmployeeCommand);
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (Exception exception)
        {
            if (exception is DomainValidationException)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return BadRequest(ModelState);
            }

            throw;
        }
    }
}

public class CreateEmployeeModel
{
    [Required]
    [MaxLength(50, ErrorMessage = "The {0} can't be more than {1} characters.")]
    [MinLength(4, ErrorMessage = "The {0} must be at least {1} characters.")]
    public string Name { get; set; }

    [Required]
    public Guid DepartmentId { get; set; }

    [Required]
    [MinAge(15, 0, 0, ErrorMessage = "The minimum age has to be 15 years.")]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [MaxLength(50, ErrorMessage = "The {0} can't be more than {1} characters.")]
    [MinLength(8, ErrorMessage = "The {0} must be at least {1} characters.")]
    public string Email { get; set; }

    [Required]
    [MaxLength(15, ErrorMessage = "The {0} can't be more than {1} characters.")]
    [MinLength(10, ErrorMessage = "The {0} must be at least {1} characters.")]
    public string PhoneNumber { get; set; }
}
