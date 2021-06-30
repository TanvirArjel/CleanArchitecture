using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.ApiModels.EmployeeModels
{
    public class UpdateEmployeeModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [MinLength(4)]
        public string Name { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int DepartmentId { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(50)]
        [MinLength(8)]
        public string Email { get; set; }

        [Required]
        [MaxLength(15)]
        [MinLength(10)]
        public string PhoneNumber { get; set; }
    }
}
