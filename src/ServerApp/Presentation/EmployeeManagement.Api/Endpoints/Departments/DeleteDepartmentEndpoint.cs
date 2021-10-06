using System;
using System.Threading.Tasks;
using EmployeeManagement.Api.EndpointBases;
using EmployeeManagement.Application.Commands.DepartmentCommands;
using EmployeeManagement.Application.Queries.DepartmentQueries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EmployeeManagement.Api.Endpoints.Departments
{
    public class DeleteDepartmentEndpoint : DepartmentEndpoint
    {
        private readonly IMediator _mediator;

        public DeleteDepartmentEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpDelete("{departmentId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Delete an existing department by department id.")]
        public async Task<IActionResult> Delete(Guid departmentId)
        {
            if (departmentId == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, $"The value of {nameof(departmentId)} must be not empty.");
                return BadRequest(ModelState);
            }

            IsDepartmentExistentByIdQuery query = new IsDepartmentExistentByIdQuery(departmentId);

            bool isExistent = await _mediator.Send(query);

            if (isExistent == false)
            {
                ModelState.AddModelError(nameof(departmentId), "The Department does not exist.");
                return BadRequest(ModelState);
            }

            DeleteDepartmentCommand comand = new DeleteDepartmentCommand(departmentId);

            await _mediator.Send(comand);
            return NoContent();
        }
    }
}
