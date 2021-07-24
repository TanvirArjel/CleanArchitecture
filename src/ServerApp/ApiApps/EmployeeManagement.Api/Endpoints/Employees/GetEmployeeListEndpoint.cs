using System.Threading.Tasks;
using EmployeeManagement.Api.Controllers;
using EmployeeManagement.Application.Dtos.EmployeeDtos;
using EmployeeManagement.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TanvirArjel.EFCore.GenericRepository;

namespace EmployeeManagement.Api.Endpoints.Employees
{
    public class GetEmployeeListEndpoint : EmployeeEndpoint
    {
        private readonly IEmployeeService _employeeService;

        public GetEmployeeListEndpoint(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // GET: api/employees
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Get the employee paginated list by page number and page size.")]
        public async Task<ActionResult<PaginatedList<EmployeeDetailsDto>>> Get(int pageNumber, int pageSize)
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
    }
}
