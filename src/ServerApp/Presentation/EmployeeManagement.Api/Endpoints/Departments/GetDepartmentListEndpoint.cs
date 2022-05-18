using EmployeeManagement.Application.Queries.DepartmentQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EmployeeManagement.Api.Endpoints.Departments;

public class GetDepartmentListEndpoint : DepartmentEndpointBase
{
    private readonly IMediator _mediator;

    public GetDepartmentListEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Get the list of all departments.")]
    public async Task<ActionResult<List<DepartmentDto>>> Get()
    {
        GetDepartmentListQuery departmentListQuery = new GetDepartmentListQuery();
        List<DepartmentDto> departmentDetailsDtos = await _mediator.Send(departmentListQuery);
        return departmentDetailsDtos;
    }
}
