using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BlazorWasmApp.ViewModels.DepartmentsViewModels
{
    public class UpdateDepartmentViewModel
    {
        public int DepartmentId { get; set; }

        [Required]
        [DisplayName("Department Name")]
        [MinLength(2, ErrorMessage = "{0} should be at least {1} characters long.")]
        [MaxLength(20, ErrorMessage = "{0} should not be more than {1} characters.")]
        public string DepartmentName { get; set; }

        [Required]
        [DisplayName("Description")]
        [MinLength(20, ErrorMessage = "{0} should be at least {1} characters long.")]
        [MaxLength(200, ErrorMessage = "{0} should not be more than {1} characters.")]
        public string Description { get; set; }

        public bool IsActive { get; set; }
    }
}
