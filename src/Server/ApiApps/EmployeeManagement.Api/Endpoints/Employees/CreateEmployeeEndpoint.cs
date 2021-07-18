using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using EmployeeManagement.Api.Controllers;
using EmployeeManagement.Application.Dtos.EmployeeDtos;
using EmployeeManagement.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TanvirArjel.CustomValidation.Attributes;

namespace EmployeeManagement.Api.Endpoints.Employees
{
    public class CreateEmployeeEndpoint : EmployeeEndpoint
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;

        public CreateEmployeeEndpoint(
            IEmployeeService employeeService,
            IDepartmentService departmentService)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
        }

        // POST: api/employees
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Create a new employee by posting the required data.")]
        public async Task<ActionResult> Post([FromBody] CreateEmployeeModel model)
        {
            bool isExistent = await _departmentService.ExistsAsync(model.DepartmentId);

            if (!isExistent)
            {
                ModelState.AddModelError(nameof(model.DepartmentId), "The Department does not exist.");
                return BadRequest(ModelState);
            }

            CreateEmployeeDto createEmployeeDto = new CreateEmployeeDto()
            {
                Name = model.Name,
                DepartmentId = model.DepartmentId,
                DateOfBirth = model.DateOfBirth,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            await _employeeService.CreateAsync(createEmployeeDto);
            return CreatedAtAction(nameof(GetEmployeeDetailsByIdEndpoint), new { employeeId = 1 }, createEmployeeDto);
        }
    }

    public class CreateEmployeeModel
    {
        [Required]
        [MaxLength(50, ErrorMessage = "The {0} can't be more than {1} characters.")]
        [MinLength(4, ErrorMessage = "The {0} must be at least {1} characters.")]
        public string Name { get; set; }

        [Required]
        public Guid DepartmentId { get; set; }

        [Required]
        [MinAge(15, 0, 0, ErrorMessage = "The minimum age has to be 15 years.")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "The {0} can't be more than {1} characters.")]
        [MinLength(8, ErrorMessage = "The {0} must be at least {1} characters.")]
        public string Email { get; set; }

        [Required]
        [MaxLength(15, ErrorMessage = "The {0} can't be more than {1} characters.")]
        [MinLength(10, ErrorMessage = "The {0} must be at least {1} characters.")]
        public string PhoneNumber { get; set; }
    }
}
