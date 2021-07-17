using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/employees")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(GroupName = "Employee Endpoints")]
    public class EmployeeEndpoint : ControllerBase
    {
    }
}
