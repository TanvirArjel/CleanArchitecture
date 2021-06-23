using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.AutoMapper
{
    public class UpdateDepartmentModel
    {
        [Required]
        public int DepartmentId { get; set; }

        [Required]
        [MaxLength(20)]
        [MinLength(2)]
        public string DepartmentName { get; set; }

        [MaxLength(200)]
        [MinLength(20)]
        public string Description { get; set; }
    }
}
