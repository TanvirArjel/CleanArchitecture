using System.ComponentModel.DataAnnotations;
using EmployeeManagement.Application.Commands.DepartmentCommands;
using EmployeeManagement.Application.Queries.DepartmentQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EmployeeManagement.Api.Endpoints.Departments;

public class CreateDepartmentEndpoint : DepartmentEndpointBase
{
    private readonly IMediator _mediator;

    public CreateDepartmentEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Create a new department by posting the required data.")]
    public async Task<ActionResult> Post(CreateDepartmentModel model)
    {
        IsDepartmentExistentByNameQuery query = new IsDepartmentExistentByNameQuery(model.Name);
        bool isNameAlreadyExistent = await _mediator.Send(query);

        if (isNameAlreadyExistent)
        {
            ModelState.AddModelError(nameof(model.Name), "The Name already exists.");
            return BadRequest(ModelState);
        }

        CreateDepratmentCommand command = new CreateDepratmentCommand(model.Name, model.Description);

        Guid departmentId = await _mediator.Send(command);
        return Created($"/api/v1/departments/{departmentId}", model);
    }
}

public class CreateDepartmentModel
{
    [Required]
    [MaxLength(20, ErrorMessage = "The {0} can't be more than {1} characters.")]
    [MinLength(2, ErrorMessage = "The {0} must be at least {1} characters.")]
    public string Name { get; set; }

    [MaxLength(200, ErrorMessage = "The {0} can't be more than {1} characters.")]
    [MinLength(20, ErrorMessage = "The {0} must be at least {1} characters.")]
    public string Description { get; set; }
}
