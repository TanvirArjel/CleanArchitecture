using CleanHr.Application.Queries.DepartmentQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Features.Department.Endpoints;

public sealed class GetDepartmentSelectListEndpoint : DepartmentEndpointBase
{
    private readonly IMediator _mediator;

    public GetDepartmentSelectListEndpoint(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet("select-list")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Get the department select list.")]
    public async Task<ActionResult<SelectList>> Get(Guid? selectedDepartment)
    {
        if (selectedDepartment == Guid.Empty)
        {
            ModelState.AddModelError(nameof(selectedDepartment), $"The value of {nameof(selectedDepartment)} can't be empty.");
            return ValidationProblem(ModelState);
        }

        GetDepartmentListQuery departmentListQuery = new();
        List<DepartmentDto> departmentDtos = await _mediator.Send(departmentListQuery, HttpContext.RequestAborted);

        SelectList selectList = new(departmentDtos, "Id", "Name", selectedDepartment);
        return selectList;
    }
}
