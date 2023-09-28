﻿using CleanHr.Application.Queries.EmployeeQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Features.Employee.Endpoints;

public class GetEmployeeDetailsByIdEndpoint : EmployeeEndpointBase
{
    private readonly IMediator _mediator;

    public GetEmployeeDetailsByIdEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/employees/5
    [HttpGet("{employeeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Get details of an employee by employee id.")]
    public async Task<ActionResult<EmployeeDetailsDto>> Get(Guid employeeId)
    {
        if (employeeId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(employeeId), $"The value of {nameof(employeeId)} can't be empty.");
            return ValidationProblem(ModelState);
        }

        GetEmployeeByIdQuery getEmployeeByIdQuery = new GetEmployeeByIdQuery(employeeId);

        EmployeeDetailsDto employeeDetailsDto = await _mediator.Send(getEmployeeByIdQuery);
        return employeeDetailsDto;
    }
}
