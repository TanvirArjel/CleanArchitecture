using System;
using System.ComponentModel.DataAnnotations;

namespace CleanHr.Api.Features.Department.Models;

public sealed class UpdateDepartmentModel : DepartmentBaseModel
{
    [Required]
    public Guid Id { get; set; }
}
