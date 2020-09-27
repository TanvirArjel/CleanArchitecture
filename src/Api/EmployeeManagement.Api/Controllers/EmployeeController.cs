using System.Threading.Tasks;
using AutoMapper;
using EmployeeManagement.Api.ApiModels.EmployeeModels;
using EmployeeManagement.Application.Dtos.EmployeeDtos;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Domain.Dtos;
using EmployeeManagement.Domain.Dtos.EmployeeDtos;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers
{
    [Route("api/employees")]
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

        // GET: api/employees
        [HttpGet]
        public async Task<PaginatedList<EmployeeDetailsModel>> GetEmployeeList(int pageNumber, int pageSize)
        {
            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            pageSize = pageSize == 0 ? 10 : pageSize;
            PaginatedList<EmployeeDetailsDto> employeeDetailsDtoList = await _employeeService.GetEmployeeListAsync(pageNumber, pageSize);
            PaginatedList<EmployeeDetailsModel> employeeList = _mapper.Map<PaginatedList<EmployeeDetailsModel>>(employeeDetailsDtoList);
            return employeeList;
        }

        // GET: api/employees/5
        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetEmployeeDetails(int employeeId)
        {
            EmployeeDetailsDto employeeDetailsDto = await _employeeService.GetEmployeeDetailsAsync(employeeId);
            EmployeeDetailsModel employeeDetailsModel = _mapper.Map<EmployeeDetailsModel>(employeeDetailsDto);
            return Ok(employeeDetailsModel);
        }

        // POST: api/employees
        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeModel createEmployeeModel)
        {
            CreateEmployeeDto createEmployeeDto = _mapper.Map<CreateEmployeeDto>(createEmployeeModel);
            await _employeeService.CreateEmployeeAsync(createEmployeeDto);
            return Ok();
        }

        // PUT: api/employees/5
        [HttpPut("{employeeId}")]
        public async Task<IActionResult> UpdateEmployee([FromBody] UpdateEmployeeModel updateEmployeeModel)
        {
            UpdateEmployeeDto updateEmployeeDto = _mapper.Map<UpdateEmployeeDto>(updateEmployeeModel);
            await _employeeService.UpdateEmplyeeAsync(updateEmployeeDto);
            return Ok();
        }

        // DELETE: api/employees/5
        [HttpDelete("{employeeId}")]
        public async Task<IActionResult> DeleteEmployee(int employeeId)
        {
            await _employeeService.DeleteEmployee(employeeId);
            return Ok();
        }
    }
}
