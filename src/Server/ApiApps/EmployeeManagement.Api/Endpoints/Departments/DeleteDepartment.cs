using System;
using System.Threading.Tasks;
using EmployeeManagement.Api.EndpointBases;
using EmployeeManagement.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EmployeeManagement.Api.Endpoints.Departments
{
    public class DeleteDepartment : DepartmentEndpoint
    {
        private readonly IDepartmentService _departmentService;

        public DeleteDepartment(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
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

            bool isExistent = await _departmentService.ExistsAsync(departmentId);

            if (isExistent == false)
            {
                ModelState.AddModelError(nameof(departmentId), "The Department does not exist.");
                return BadRequest(ModelState);
            }

            await _departmentService.DeleteAsync(departmentId);
            return NoContent();
        }
    }
}
