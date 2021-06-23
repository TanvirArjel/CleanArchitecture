using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeeManagement.Api.ApiModels.DepartmentModels;
using EmployeeManagement.Api.AutoMapper;
using EmployeeManagement.Application.Dtos.DepartmentDtos;
using EmployeeManagement.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Swashbuckle.AspNetCore.Annotations;

namespace EmployeeManagement.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/departments")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Get the list of all departments.")]
        public async Task<ActionResult<List<DepartmentDetailsDto>>> GetDepartmentList()
        {
            List<DepartmentDetailsDto> departmentDetailsDtos = await _departmentService.GetListAsync();
            return departmentDetailsDtos;
        }

        [HttpGet("select-list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Get the department select list.")]
        public async Task<ActionResult<SelectList>> GetDepartmentSelectList(int? selectedDepartment)
        {
            if (selectedDepartment <= 0)
            {
                return BadRequest($"The value of {nameof(selectedDepartment)} msut be greater than 0.");
            }

            SelectList selectList = await _departmentService.GetSelectListAsync(selectedDepartment);
            return selectList;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Create a new department by posting the required data.")]
        public async Task<ActionResult> CreateDepartment([FromBody] CreateDepartmentModel model)
        {
            bool isNameAlreadyExistent = await _departmentService.ExistsByNameAsync(model.DepartmentName);

            if (isNameAlreadyExistent)
            {
                ModelState.AddModelError(nameof(model.DepartmentName), "The Name already exists.");
                return BadRequest(ModelState);
            }

            CreateDepartmentDto createDepartmentDto = new CreateDepartmentDto
            {
                DepartmentName = model.DepartmentName,
                Description = model.Description
            };

            int departmentId = await _departmentService.CreateAsync(createDepartmentDto);
            return CreatedAtAction(nameof(GetDepartment), new { departmentId }, model);
        }

        [HttpGet("{departmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Get the details of a department by department id.")]
        public async Task<ActionResult<DepartmentDetailsDto>> GetDepartment(int departmentId)
        {
            if (departmentId <= 0)
            {
                return BadRequest($"The value of {nameof(departmentId)} msut be greater than 0.");
            }

            DepartmentDetailsDto departmentDetailsDto = await _departmentService.GetByIdAsync(departmentId);
            return departmentDetailsDto;
        }

        [HttpPut("{departmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Update an existing employee by employee id and posting updated data.")]
        public async Task<ActionResult> UpdateDepartment(int departmentId, UpdateDepartmentModel model)
        {
            if (departmentId != model.DepartmentId)
            {
                ModelState.AddModelError(nameof(model.DepartmentId), "The DepartmentId does not match with route value.");
                return BadRequest(ModelState);
            }

            bool isExistent = await _departmentService.ExistsAsync(departmentId);

            if (!isExistent)
            {
                ModelState.AddModelError(nameof(model.DepartmentId), "The Department does not exist.");
                return BadRequest(ModelState);
            }

            bool isUnique = await _departmentService.IsUniqueAsync(departmentId, model.DepartmentName);

            if (isUnique == false)
            {
                ModelState.AddModelError(nameof(model.DepartmentName), "The Name already exists.");
                return BadRequest(ModelState);
            }

            UpdateDepartmentDto updateDepartmentDto = new UpdateDepartmentDto()
            {
                DepartmentId = model.DepartmentId,
                DepartmentName = model.DepartmentName,
                Description = model.Description
            };

            await _departmentService.UpdateAsync(updateDepartmentDto);
            return Ok();
        }

        [HttpDelete("{departmentId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Delete an existing department by department id.")]
        public async Task<IActionResult> DeleteDepartment(int departmentId)
        {
            if (departmentId <= 0)
            {
                ModelState.AddModelError(string.Empty, $"The value of {nameof(departmentId)} msut be greater than 0.");
                return BadRequest(ModelState);
            }

            bool isExistent = await _departmentService.ExistsAsync(departmentId);

            if (isExistent == false)
            {
                ModelState.AddModelError(nameof(departmentId), "The Department does not exist.");
                return BadRequest(ModelState);
            }

            await _departmentService.DeleteAsync(departmentId);
            return NoContent();
        }
    }
}