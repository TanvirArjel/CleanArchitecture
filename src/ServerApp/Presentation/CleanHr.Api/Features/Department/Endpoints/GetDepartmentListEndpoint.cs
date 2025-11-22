using CleanHr.Application.Queries.DepartmentQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Features.Department.Endpoints;

public sealed class GetDepartmentListEndpoint : DepartmentEndpointBase
{
    private readonly IMediator _mediator;

    public GetDepartmentListEndpoint(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Get the list of all departments.")]
    public async Task<ActionResult<List<DepartmentDto>>> Get()
    {
        GetDepartmentListQuery departmentListQuery = new();
        List<DepartmentDto> departmentDetailsDtos = await _mediator.Send(departmentListQuery, HttpContext.RequestAborted);
        return departmentDetailsDtos;
    }
}
