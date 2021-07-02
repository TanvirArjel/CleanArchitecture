using System;
using System.Threading.Tasks;
using EmployeeManagement.Api.ApiModels.EmployeeModels;
using EmployeeManagement.Application.Dtos.EmployeeDtos;
using EmployeeManagement.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TanvirArjel.EFCore.GenericRepository;

namespace EmployeeManagement.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/employees")]
    [ApiController]
    ////[Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;

        public EmployeeController(
            IEmployeeService employeeService,
            IDepartmentService departmentService)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
        }

        // GET: api/employees
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Get the employee paginated list by page number and page size.")]
        public async Task<ActionResult<PaginatedList<EmployeeDetailsDto>>> GetEmployeeList(int pageNumber, int pageSize)
        {
            if (pageNumber < 0)
            {
                return BadRequest($"The {nameof(pageNumber)} must be greater than 0.");
            }

            if (pageSize < 0)
            {
                return BadRequest($"The {nameof(pageSize)} must be in between 1 and 50.");
            }

            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            pageSize = pageSize == 0 ? 10 : pageSize;
            PaginatedList<EmployeeDetailsDto> employeeList = await _employeeService.GetListAsync(pageNumber, pageSize);
            return employeeList;
        }

        // GET: api/employees/5
        [HttpGet("{employeeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Get details of an employee by employee id.")]
        public async Task<ActionResult<EmployeeDetailsDto>> GetDetailsById(Guid employeeId)
        {
            if (employeeId == Guid.Empty)
            {
                return BadRequest($"The value of {nameof(employeeId)} can't be empty.");
            }

            EmployeeDetailsDto employeeDetailsDto = await _employeeService.GetDetailsByIdAsync(employeeId);
            return employeeDetailsDto;
        }

        // POST: api/employees
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Create a new employee by posting the required data.")]
        public async Task<ActionResult> CreateEmployee([FromBody] CreateEmployeeModel model)
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
            return CreatedAtAction(nameof(GetDetailsById), new { employeeId = 1 }, createEmployeeDto);
        }

        // PUT: api/employees/5
        [HttpPut("{employeeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Update an existing employee by employee id and posting the updated data.")]
        public async Task<ActionResult> UpdateEmployee(Guid employeeId, [FromBody] UpdateEmployeeModel model)
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

        // DELETE: api/employees/5
        [HttpDelete("{employeeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Delete an existing employee by employee id.")]
        public async Task<ActionResult> DeleteEmployee(Guid employeeId)
        {
            if (employeeId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(employeeId), $"The value of {nameof(employeeId)} can't be empty.");
                return BadRequest(ModelState);
            }

            EmployeeDetailsDto employee = await _employeeService.GetDetailsByIdAsync(employeeId);

            if (employee == null)
            {
                ModelState.AddModelError(nameof(employeeId), "The Employee does not exist.");
                return BadRequest(ModelState);
            }

            await _employeeService.DeleteAsync(employeeId);
            return Ok();
        }
    }
}
