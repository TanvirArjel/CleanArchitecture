using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeeManagement.Api.EndpointBases;
using EmployeeManagement.Application.Dtos.DepartmentDtos;
using EmployeeManagement.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EmployeeManagement.Api.Endpoints.Departments
{
    public class GetDepartmentListEndpoint : DepartmentEndpoint
    {
        private readonly IDepartmentService _departmentService;

        public GetDepartmentListEndpoint(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Get the list of all departments.")]
        public async Task<ActionResult<List<DepartmentDetailsDto>>> Get()
        {
            List<DepartmentDetailsDto> departmentDetailsDtos = await _departmentService.GetListAsync();
            return departmentDetailsDtos;
        }
    }
}
