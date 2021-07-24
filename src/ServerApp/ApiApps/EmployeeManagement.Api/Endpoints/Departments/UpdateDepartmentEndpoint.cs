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
    public class UpdateDepartmentEndpoint : DepartmentEndpoint
    {
        private readonly IDepartmentService _departmentService;

        public UpdateDepartmentEndpoint(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpPut("{departmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Update an existing employee by employee id and posting updated data.")]
        public async Task<ActionResult> Put(Guid departmentId, UpdateDepartmentModel model)
        {
            if (departmentId != model.Id)
            {
                ModelState.AddModelError(nameof(model.Id), "The DepartmentId does not match with route value.");
                return BadRequest(ModelState);
            }

            bool isExistent = await _departmentService.ExistsAsync(departmentId);

            if (!isExistent)
            {
                ModelState.AddModelError(nameof(model.Id), "The Department does not exist.");
                return BadRequest(ModelState);
            }

            bool isUnique = await _departmentService.IsUniqueAsync(departmentId, model.Name);

            if (isUnique == false)
            {
                ModelState.AddModelError(nameof(model.Name), "The Name already exists.");
                return BadRequest(ModelState);
            }

            UpdateDepartmentDto updateDepartmentDto = new UpdateDepartmentDto()
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description
            };

            await _departmentService.UpdateAsync(updateDepartmentDto);
            return Ok();
        }
    }

    public class UpdateDepartmentModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(20, ErrorMessage = "The {0} can't be more than {1} characters.")]
        [MinLength(2, ErrorMessage = "The {0} must be at least {1} characters.")]
        public string Name { get; set; }

        [MaxLength(200, ErrorMessage = "The {0} can't be more than {1} characters.")]
        [MinLength(20, ErrorMessage = "The {0} must be at least {1} characters.")]
        public string Description { get; set; }
    }
}
