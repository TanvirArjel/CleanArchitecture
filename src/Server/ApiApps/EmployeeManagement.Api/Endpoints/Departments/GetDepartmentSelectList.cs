using System;
using System.Threading.Tasks;
using EmployeeManagement.Api.EndpointBases;
using EmployeeManagement.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Swashbuckle.AspNetCore.Annotations;

namespace EmployeeManagement.Api.Endpoints.Departments
{
    public class GetDepartmentSelectList : DepartmentEndpoint
    {
        private readonly IDepartmentService _departmentService;

        public GetDepartmentSelectList(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet("select-list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Get the department select list.")]
        public async Task<ActionResult<SelectList>> Get(Guid? selectedDepartment)
        {
            if (selectedDepartment == Guid.Empty)
            {
                return BadRequest($"The value of {nameof(selectedDepartment)} can't be empty.");
            }

            SelectList selectList = await _departmentService.GetSelectListAsync(selectedDepartment);
            return selectList;
        }
    }
}
