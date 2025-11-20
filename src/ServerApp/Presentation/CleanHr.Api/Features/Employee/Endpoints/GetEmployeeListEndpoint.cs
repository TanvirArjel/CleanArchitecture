using CleanHr.Application.Queries.EmployeeQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Api.Features.Employee.Endpoints;

public sealed class GetEmployeeListEndpoint(IMediator mediator) : EmployeeEndpointBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    // GET: api/employees
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Get the employee paginated list by page number and page size.")]
    public async Task<ActionResult<PaginatedList<EmployeeDto>>> Get(int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
        {
            ModelState.AddModelError(nameof(pageNumber), $"The {nameof(pageNumber)} must be greater than 0.");
            return ValidationProblem(ModelState);
        }

        if (pageSize < 1 || pageSize > 50)
        {
            ModelState.AddModelError(nameof(pageSize), $"The {nameof(pageSize)} must be in between 1 and 50.");
            return ValidationProblem(ModelState);
        }

        GetEmployeeListQuery getEmployeeListQuery = new(pageNumber, pageSize);
        PaginatedList<EmployeeDto> employeeList = await _mediator.Send(getEmployeeListQuery, HttpContext.RequestAborted);
        return employeeList;
    }
}
