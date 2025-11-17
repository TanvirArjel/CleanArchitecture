using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CleanHr.Blazor.Models.DepartmentModels;

public class UpdateDepartmentModel
{
    public Guid Id { get; set; }

    [Required]
    [DisplayName("Department Name")]
    [MinLength(2, ErrorMessage = "{0} should be at least {1} characters long.")]
    [MaxLength(20, ErrorMessage = "{0} should not be more than {1} characters.")]
    public string Name { get; set; }

    [Required]
    [DisplayName("Description")]
    [MinLength(20, ErrorMessage = "{0} should be at least {1} characters long.")]
    [MaxLength(200, ErrorMessage = "{0} should not be more than {1} characters.")]
    public string Description { get; set; }

    public bool IsActive { get; set; }
}
