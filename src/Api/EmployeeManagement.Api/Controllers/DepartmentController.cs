using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EmployeeManagement.Api.ApiModels.DepartmentModels;
using EmployeeManagement.Api.AutoMapper;
using EmployeeManagement.Application.Dtos.DepartmentDtos;
using EmployeeManagement.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeManagement.Api.Controllers
{
    [Route("api/departments")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly IMapper _mapper;

        public DepartmentController(IDepartmentService departmentService, IMapper mapper)
        {
            _departmentService = departmentService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<List<DepartmentDetailsModel>> GetDepartmentList()
        {
            List<DepartmentDetailsDto> departmentDetailsDtos = await _departmentService.GetDepartmentListAsync();
            List<DepartmentDetailsModel> departmentList = _mapper.Map<List<DepartmentDetailsModel>>(departmentDetailsDtos);
            return departmentList;
        }

        [HttpGet("select-list")]
        public async Task<SelectList> GetDepartmentSelectList(int? selectedDepartment)
        {
            SelectList selectList = await _departmentService.GetDepartmentSelectListAsync(selectedDepartment);
            return selectList;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentModel createDepartmentModel)
        {
            CreateDepartmentDto createDepartmentDto = _mapper.Map<CreateDepartmentDto>(createDepartmentModel);
            await _departmentService.CreateDepartmentAsync(createDepartmentDto);
            return Ok();
        }

        [HttpGet("{departmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetDepartment(int departmentId)
        {
            DepartmentDetailsDto departmentDetailsDto = await _departmentService.GetDepartmentAsync(departmentId);

            if (departmentDetailsDto == null)
            {
                return NotFound();
            }

            DepartmentDetailsModel departmentDetailsModel = _mapper.Map<DepartmentDetailsModel>(departmentDetailsDto);
            return Ok(departmentDetailsModel);
        }

        [HttpPut("{departmentId}")]
        public async Task<IActionResult> UpdateDepartment([FromBody] UpdateDepartmentModel updateDepartmentModel)
        {
            UpdateDepartmentDto updateDepartmentDto = _mapper.Map<UpdateDepartmentDto>(updateDepartmentModel);
            await _departmentService.UpdateDepartmentAsync(updateDepartmentDto);
            return Ok();
        }

        [HttpDelete("{departmentId}")]
        public async Task<IActionResult> DeleteDepartment(int departmentId)
        {
            await _departmentService.DeleteDepartment(departmentId);
            return Ok();
        }
    }
}