using System.ComponentModel.DataAnnotations;

namespace CleanHr.Api.Features.Department.Models;

public sealed class UpdateDepartmentModel : DepartmentBaseModel
{
    public Guid Id { get; set; }
}
