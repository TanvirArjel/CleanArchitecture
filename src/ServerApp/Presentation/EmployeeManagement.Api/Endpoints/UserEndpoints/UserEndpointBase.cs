using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Endpoints.UserEndpoints;

[Authorize]
[Route("api/v{version:apiVersion}/user")]
[ApiController]
[ApiExplorerSettings(GroupName = "User Endpoints")]
public abstract class UserEndpointBase : ControllerBase
{
}
