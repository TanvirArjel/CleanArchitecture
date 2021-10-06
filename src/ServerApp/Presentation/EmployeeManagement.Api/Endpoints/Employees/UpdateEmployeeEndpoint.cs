using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using EmployeeManagement.Application.Commands.EmployeeCommands;
using EmployeeManagement.Application.Queries.DepartmentQueries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EmployeeManagement.Api.Endpoints.Employees
{
    public class UpdateEmployeeEndpoint : EmployeeEndpointBase
    {
        private readonly IMediator _mediator;

        public UpdateEmployeeEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        // PUT: api/employees/5
        [HttpPut("{employeeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Update an existing employee by employee id and posting the updated data.")]
        public async Task<ActionResult> Put(Guid employeeId, [FromBody] UpdateEmployeeModel model)
        {
            IsDepartmentExistentByIdQuery isDepartmentExistentById = new IsDepartmentExistentByIdQuery(employeeId);

            bool isExistent = await _mediator.Send(isDepartmentExistentById);

            if (!isExistent)
            {
                ModelState.AddModelError(nameof(model.DepartmentId), "The Department does not exist.");
                return BadRequest(ModelState);
            }

            UpdateEmployeeCommand command = new UpdateEmployeeCommand(
                employeeId,
                model.Name,
                model.DepartmentId,
                model.DateOfBirth,
                model.Email,
                model.PhoneNumber);

            await _mediator.Send(command);
            return Ok();
        }
    }

    public class UpdateEmployeeModel
    {
        [Required]
        [MaxLength(50)]
        [MinLength(4)]
        public string Name { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public Guid DepartmentId { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(50)]
        [MinLength(8)]
        public string Email { get; set; }

        [Required]
        [MaxLength(15)]
        [MinLength(10)]
        public string PhoneNumber { get; set; }
    }
}
