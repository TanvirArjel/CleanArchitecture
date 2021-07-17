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
    public class GetEmployeeDetailsById : EmployeeEndpoint
    {
        private readonly IEmployeeService _employeeService;

        public GetEmployeeDetailsById(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // GET: api/employees/5
        [HttpGet("{employeeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Get details of an employee by employee id.")]
        public async Task<ActionResult<EmployeeDetailsDto>> Get(Guid employeeId)
        {
            if (employeeId == Guid.Empty)
            {
                return BadRequest($"The value of {nameof(employeeId)} can't be empty.");
            }

            EmployeeDetailsDto employeeDetailsDto = await _employeeService.GetDetailsByIdAsync(employeeId);
            return employeeDetailsDto;
        }
    }
}
