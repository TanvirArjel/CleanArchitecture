using CleanHr.Api.Features.Employee.Models;
using CleanHr.Application.Commands.EmployeeCommands;
using CleanHr.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Features.Employee.Endpoints;

public class UpdateEmployeeEndpoint : EmployeeEndpointBase
{
    private readonly IMediator _mediator;

    public UpdateEmployeeEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    // PUT: api/v1/employees/{Guid}
    [HttpPut("{employeeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Update an existing employee by employee id and posting the updated data.")]
    public async Task<ActionResult> Put(Guid employeeId, UpdateEmployeeModel model)
    {
        try
        {
            if (employeeId == Guid.Empty)
            {
                ModelState.AddModelError("employeeId", "The employeeId cannot be empty guid.");
                return ValidationProblem(ModelState);
            }

            UpdateEmployeeCommand command = new UpdateEmployeeCommand(
                employeeId,
                model.Name,
                model.DepartmentId,
                model.DateOfBirth,
                model.Email,
                model.PhoneNumber);

            await _mediator.Send(command);
            return Ok();
        }
        catch (Exception exception)
        {
            if (exception is EntityNotFoundException)
            {
                ModelState.AddModelError(nameof(model.DepartmentId), "The Department does not exist.");
                return ValidationProblem(ModelState);
            }

            if (exception is DomainValidationException)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return ValidationProblem(ModelState);
            }

            throw;
        }
    }
}
