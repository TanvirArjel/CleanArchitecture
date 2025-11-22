using CleanHr.Application.Queries.DepartmentQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Features.Department.Endpoints;

public sealed class GetDepartmentByIdEndpoint : DepartmentEndpointBase
{
    private readonly IMediator _mediator;

    public GetDepartmentByIdEndpoint(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet("{departmentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Get the details of a department by department id.")]
    public async Task<ActionResult<DepartmentDetailsDto>> GetDepartment(Guid departmentId)
    {
        if (departmentId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(departmentId), $"The value of {nameof(departmentId)} can't be empty.");
            return ValidationProblem(ModelState);
        }

        GetDepartmentByIdQuery query = new(departmentId);

        DepartmentDetailsDto departmentDetailsDto = await _mediator.Send(query, HttpContext.RequestAborted);
        return departmentDetailsDto;
    }
}
