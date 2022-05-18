using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Endpoints.Employees;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/employees")]
[Authorize]
[ApiExplorerSettings(GroupName = "Employee Endpoints")]
public abstract class EmployeeEndpointBase : ControllerBase
{
}
