using System;
using System.Threading.Tasks;
using EmployeeManagement.Api.EndpointBases;
using EmployeeManagement.Application.Dtos.DepartmentDtos;
using EmployeeManagement.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EmployeeManagement.Api.Endpoints.Departments
{
    public class GetDepartmentByIdEndpoint : DepartmentEndpoint
    {
        private readonly IDepartmentService _departmentService;

        public GetDepartmentByIdEndpoint(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet("{departmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Get the details of a department by department id.")]
        public async Task<ActionResult<DepartmentDetailsDto>> GetDepartment(Guid departmentId)
        {
            if (departmentId == Guid.Empty)
            {
                return BadRequest($"The value of {nameof(departmentId)} can't be empty.");
            }

            DepartmentDetailsDto departmentDetailsDto = await _departmentService.GetByIdAsync(departmentId);
            return departmentDetailsDto;
        }
    }
}
