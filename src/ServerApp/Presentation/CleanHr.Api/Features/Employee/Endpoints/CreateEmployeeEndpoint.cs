using CleanHr.Api.Features.Employee.Models;
using CleanHr.Application.Commands.EmployeeCommands;
using CleanHr.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Features.Employee.Endpoints;

public class CreateEmployeeEndpoint(IMediator mediator) : EmployeeEndpointBase
{
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
            CreateEmployeeCommand createEmployeeCommand = new(
                model.Name,
                model.Name,
                model.DepartmentId,
                model.DateOfBirth,
                model.Email,
                model.PhoneNumber);

            await mediator.Send(createEmployeeCommand, HttpContext.RequestAborted);
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (Exception exception)
        {
            if (exception is DomainValidationException)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return ValidationProblem(ModelState);
            }

            throw;
        }
    }
}
