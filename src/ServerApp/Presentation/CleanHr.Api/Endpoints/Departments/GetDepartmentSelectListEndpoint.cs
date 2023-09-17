using CleanHr.Application.Queries.DepartmentQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Endpoints.Departments;

public class GetDepartmentSelectListEndpoint(IMediator mediator) : DepartmentEndpointBase
{
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

        GetDepartmentListQuery departmentListQuery = new GetDepartmentListQuery();
        List<DepartmentDto> departmentDtos = await mediator.Send(departmentListQuery);

        SelectList selectList = new SelectList(departmentDtos, "Id", "Name", selectedDepartment);
        return selectList;
    }
}
