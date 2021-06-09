using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.ApiModels.DepartmentModels
{
    public class CreateDepartmentModel
    {
        [Required]
        [MaxLength(20)]
        [MinLength(2)]
        public string DepartmentName { get; set; }

        [MaxLength(200)]
        [MinLength(20)]
        public string Description { get; set; }
    }
}
