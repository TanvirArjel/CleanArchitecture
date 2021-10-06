using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using EmployeeManagement.Api.Controllers;
using EmployeeManagement.Application.Dtos.EmployeeDtos;
using EmployeeManagement.Application.Queries.DepartmentQueries;
using EmployeeManagement.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TanvirArjel.CustomValidation.Attributes;

namespace EmployeeManagement.Api.Endpoints.Employees
{
    public class CreateEmployeeEndpoint : EmployeeEndpoint
    {
        private readonly IMediator _mediator;
        private readonly IEmployeeService _employeeService;

        public CreateEmployeeEndpoint(
            IEmployeeService employeeService,
            IMediator mediator)
        {
            _employeeService = employeeService;
            _mediator = mediator;
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
            IsDepartmentExistentByIdQuery isDepartmentExistentByIdQuery = new IsDepartmentExistentByIdQuery(model.DepartmentId);
            bool isExistent = await _mediator.Send(isDepartmentExistentByIdQuery);

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
            return StatusCode(StatusCodes.Status201Created);
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
