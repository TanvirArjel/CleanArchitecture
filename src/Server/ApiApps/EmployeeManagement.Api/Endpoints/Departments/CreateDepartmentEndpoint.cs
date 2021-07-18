using System;
using System.Threading.Tasks;
using EmployeeManagement.Api.EndpointBases;
using EmployeeManagement.Api.EndpointModels.DepartmentModels;
using EmployeeManagement.Application.Dtos.DepartmentDtos;
using EmployeeManagement.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EmployeeManagement.Api.Endpoints.Departments
{
    public class CreateDepartmentEndpoint : DepartmentEndpoint
    {
        private readonly IDepartmentService _departmentService;

        public CreateDepartmentEndpoint(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Create a new department by posting the required data.")]
        public async Task<ActionResult> Post(CreateDepartmentModel model)
        {
            bool isNameAlreadyExistent = await _departmentService.ExistsByNameAsync(model.Name);

            if (isNameAlreadyExistent)
            {
                ModelState.AddModelError(nameof(model.Name), "The Name already exists.");
                return BadRequest(ModelState);
            }

            CreateDepartmentDto createDepartmentDto = new CreateDepartmentDto
            {
                Name = model.Name,
                Description = model.Description
            };

            Guid departmentId = await _departmentService.CreateAsync(createDepartmentDto);
            return CreatedAtAction("GetDepartment", new { departmentId }, model);
        }
    }
}
