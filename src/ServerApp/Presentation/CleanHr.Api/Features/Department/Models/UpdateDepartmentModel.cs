using System.ComponentModel.DataAnnotations;

namespace CleanHr.Api.Features.Department.Models;

internal sealed class UpdateDepartmentModel : DepartmentBaseModel
{
    public Guid Id { get; set; }
}
