using Microsoft.AspNetCore.Mvc;

namespace CleanHr.Api.Features.Department.Endpoints;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/departments")]
[ApiController]
////[Authorize]
[ApiExplorerSettings(GroupName = "Department Endpoints")]
public abstract class DepartmentEndpointBase : ControllerBase
{
}
