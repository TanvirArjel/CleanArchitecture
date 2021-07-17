using System;
using System.Threading.Tasks;
using EmployeeManagement.Api.Controllers;
using EmployeeManagement.Application.Dtos.EmployeeDtos;
using EmployeeManagement.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EmployeeManagement.Api.Endpoints.Employees
{
    public class DeleteEmployee : EmployeeEndpoint
    {
        private readonly IEmployeeService _employeeService;

        public DeleteEmployee(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // DELETE: api/employees/5
        [HttpDelete("{employeeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Delete an existing employee by employee id.")]
        public async Task<ActionResult> Delete(Guid employeeId)
        {
            if (employeeId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(employeeId), $"The value of {nameof(employeeId)} can't be empty.");
                return BadRequest(ModelState);
            }

            EmployeeDetailsDto employee = await _employeeService.GetDetailsByIdAsync(employeeId);

            if (employee == null)
            {
                ModelState.AddModelError(nameof(employeeId), "The Employee does not exist.");
                return BadRequest(ModelState);
            }

            await _employeeService.DeleteAsync(employeeId);
            return Ok();
        }
    }
}
