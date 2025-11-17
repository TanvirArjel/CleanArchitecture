using CleanHr.Api.Features.Employee.Models;
using CleanHr.Application.Commands.EmployeeCommands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Features.Employee.Endpoints;

internal class UpdateEmployeeEndpoint(IMediator mediator) : EmployeeEndpointBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    // PUT: api/v1/employees/{Guid}
    [HttpPut("{employeeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Update an existing employee by employee id and posting the updated data.")]
    public async Task<ActionResult> Put(Guid employeeId, UpdateEmployeeModel model)
    {
        if (employeeId == Guid.Empty)
        {
            ModelState.AddModelError("employeeId", "The employeeId cannot be empty guid.");
            return ValidationProblem(ModelState);
        }

        UpdateEmployeeCommand command = new(
                employeeId,
                model.Name,
                model.DepartmentId,
                model.DateOfBirth,
                model.Email,
                model.PhoneNumber);

        await _mediator.Send(command, HttpContext.RequestAborted);
        return Ok();
    }
}
