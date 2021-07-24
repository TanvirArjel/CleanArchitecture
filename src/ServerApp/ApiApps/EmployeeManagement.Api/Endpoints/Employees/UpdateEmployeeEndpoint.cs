using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using EmployeeManagement.Api.Controllers;
using EmployeeManagement.Application.Dtos.EmployeeDtos;
using EmployeeManagement.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EmployeeManagement.Api.Endpoints.Employees
{
    public class UpdateEmployeeEndpoint : EmployeeEndpoint
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;

        public UpdateEmployeeEndpoint(
            IEmployeeService employeeService,
            IDepartmentService departmentService)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
        }

        // PUT: api/employees/5
        [HttpPut("{employeeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Update an existing employee by employee id and posting the updated data.")]
        public async Task<ActionResult> Put(Guid employeeId, [FromBody] UpdateEmployeeModel model)
        {
            if (employeeId != model.Id)
            {
                ModelState.AddModelError(nameof(model.Id), "The EmployeeId does not match with route value.");
                return BadRequest(ModelState);
            }

            bool isExistent = await _departmentService.ExistsAsync(model.DepartmentId);

            if (!isExistent)
            {
                ModelState.AddModelError(nameof(model.DepartmentId), "The Department does not exist.");
                return BadRequest(ModelState);
            }

            UpdateEmployeeDto updateEmployeeDto = new UpdateEmployeeDto
            {
                Id = model.Id,
                Name = model.Name,
                DepartmentId = model.DepartmentId,
                DateOfBirth = model.DateOfBirth,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            await _employeeService.UpdateAsync(updateEmployeeDto);
            return Ok();
        }
    }

    public class UpdateEmployeeModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        [MinLength(4)]
        public string Name { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public Guid DepartmentId { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(50)]
        [MinLength(8)]
        public string Email { get; set; }

        [Required]
        [MaxLength(15)]
        [MinLength(10)]
        public string PhoneNumber { get; set; }
    }
}
