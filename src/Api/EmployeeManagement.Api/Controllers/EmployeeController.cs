using AutoMapper;
using EmployeeManagement.Api.ApiModels.EmployeeModels;
using EmployeeManagement.Application.Dtos.EmployeeDtos;
using EmployeeManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagement.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public EmployeeController(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }

        // GET: api/Employee
        [HttpGet]
        public async Task<List<EmployeeDetailsModel>> GetEmployeeList()
        {
            List<EmployeeDetailsDto> employeeDetailsDtoList = await _employeeService.GetEmployeeListAsync();
            List<EmployeeDetailsModel> employeeList = _mapper.Map<List<EmployeeDetailsModel>>(employeeDetailsDtoList);
            return employeeList;
        }

        // GET: api/Employee/5
        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetEmployeeDetails(int employeeId)
        {
            EmployeeDetailsDto employeeDetailsDto = await _employeeService.GetEmployeeDetailsAsync(employeeId);
            EmployeeDetailsModel employeeDetailsModel = _mapper.Map<EmployeeDetailsModel>(employeeDetailsDto);
            return Ok(employeeDetailsModel);
        }

        // POST: api/Employee
        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeModel createEmployeeModel)
        {
            CreateEmployeeDto createEmployeeDto = _mapper.Map<CreateEmployeeDto>(createEmployeeModel);
            await _employeeService.CreateEmployeeAsync(createEmployeeDto);
            return Ok();
        }

        // PUT: api/Employee/5
        [HttpPut]
        public async Task<IActionResult> UpdateEmployee([FromBody] UpdateEmployeeModel updateEmployeeModel)
        {
            UpdateEmployeeDto updateEmployeeDto = _mapper.Map<UpdateEmployeeDto>(updateEmployeeModel);
            await _employeeService.UpdateEmplyeeAsync(updateEmployeeDto);
            return Ok();
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{employeeId}")]
        public async Task<IActionResult> DeleteEmployee(int employeeId)
        {
            await _employeeService.DeleteEmployee(employeeId);
            return Ok();
        }
    }
}
