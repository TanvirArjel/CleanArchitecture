using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using EmployeeManagement.Api.EndpointBases;
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
            return Created($"/api/v1/departments/{departmentId}", model);
        }
    }

    public class CreateDepartmentModel
    {
        [Required]
        [MaxLength(20, ErrorMessage = "The {0} can't be more than {1} characters.")]
        [MinLength(2, ErrorMessage = "The {0} must be at least {1} characters.")]
        public string Name { get; set; }

        [MaxLength(200, ErrorMessage = "The {0} can't be more than {1} characters.")]
        [MinLength(20, ErrorMessage = "The {0} must be at least {1} characters.")]
        public string Description { get; set; }
    }
}
