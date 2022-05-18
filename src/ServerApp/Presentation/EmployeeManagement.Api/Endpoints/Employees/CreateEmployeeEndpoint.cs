using System.ComponentModel.DataAnnotations;
using EmployeeManagement.Application.Commands.EmployeeCommands;
using EmployeeManagement.Application.Queries.DepartmentQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TanvirArjel.CustomValidation.Attributes;

namespace EmployeeManagement.Api.Endpoints.Employees;

public class CreateEmployeeEndpoint : EmployeeEndpointBase
{
    private readonly IMediator _mediator;

    public CreateEmployeeEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST: api/employees
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Create a new employee by posting the required data.")]
    public async Task<ActionResult> Post([FromBody] CreateEmployeeModel model)
    {
        IsDepartmentExistentByIdQuery isDepartmentExistentByIdQuery = new IsDepartmentExistentByIdQuery(model.DepartmentId);
        bool isExistent = await _mediator.Send(isDepartmentExistentByIdQuery);

        if (!isExistent)
        {
            ModelState.AddModelError(nameof(model.DepartmentId), "The Department does not exist.");
            return BadRequest(ModelState);
        }

        CreateEmployeeCommand createEmployeeCommand = new CreateEmployeeCommand(
            model.Name,
            model.DepartmentId,
            model.DateOfBirth,
            model.Email,
            model.PhoneNumber);

        await _mediator.Send(createEmployeeCommand);
        return StatusCode(StatusCodes.Status201Created);
    }
}

public class CreateEmployeeModel
{
    [Required]
    [MaxLength(50, ErrorMessage = "The {0} can't be more than {1} characters.")]
    [MinLength(4, ErrorMessage = "The {0} must be at least {1} characters.")]
    public string Name { get; set; }

    [Required]
    public Guid DepartmentId { get; set; }

    [Required]
    [MinAge(15, 0, 0, ErrorMessage = "The minimum age has to be 15 years.")]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [MaxLength(50, ErrorMessage = "The {0} can't be more than {1} characters.")]
    [MinLength(8, ErrorMessage = "The {0} must be at least {1} characters.")]
    public string Email { get; set; }

    [Required]
    [MaxLength(15, ErrorMessage = "The {0} can't be more than {1} characters.")]
    [MinLength(10, ErrorMessage = "The {0} must be at least {1} characters.")]
    public string PhoneNumber { get; set; }
}
