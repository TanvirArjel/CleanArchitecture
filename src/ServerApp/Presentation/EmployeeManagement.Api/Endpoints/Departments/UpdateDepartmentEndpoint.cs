using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using EmployeeManagement.Application.Commands.DepartmentCommands;
using EmployeeManagement.Application.Queries.DepartmentQueries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EmployeeManagement.Api.Endpoints.Departments
{
    public class UpdateDepartmentEndpoint : DepartmentEndpointBase
    {
        private readonly IMediator _mediator;

        public UpdateDepartmentEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPut("{departmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Update an existing employee by employee id and posting updated data.")]
        public async Task<ActionResult> Put(Guid departmentId, UpdateDepartmentModel model)
        {
            if (departmentId != model.Id)
            {
                ModelState.AddModelError(nameof(model.Id), "The DepartmentId does not match with route value.");
                return BadRequest(ModelState);
            }

            IsDepartmentExistentByIdQuery isExistentQuery = new IsDepartmentExistentByIdQuery(departmentId);

            bool isExistent = await _mediator.Send(isExistentQuery);

            if (!isExistent)
            {
                ModelState.AddModelError(nameof(model.Id), "The Department does not exist.");
                return BadRequest(ModelState);
            }

            IsDepartmentNameUniqueQuery uniqueQuery = new IsDepartmentNameUniqueQuery(departmentId, model.Name);

            bool isUnique = await _mediator.Send(uniqueQuery);

            if (isUnique == false)
            {
                ModelState.AddModelError(nameof(model.Name), "The Name already exists.");
                return BadRequest(ModelState);
            }

            UpdateDepartmentCommand command = new UpdateDepartmentCommand(departmentId, model.Name, model.Description, true);

            await _mediator.Send(command);
            return Ok();
        }
    }

    public class UpdateDepartmentModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(20, ErrorMessage = "The {0} can't be more than {1} characters.")]
        [MinLength(2, ErrorMessage = "The {0} must be at least {1} characters.")]
        public string Name { get; set; }

        [MaxLength(200, ErrorMessage = "The {0} can't be more than {1} characters.")]
        [MinLength(20, ErrorMessage = "The {0} must be at least {1} characters.")]
        public string Description { get; set; }
    }
}
