using System;

namespace EmployeeManagement.Application.Dtos.DepartmentDtos
{
    public class DepartmentDetailsDto
    {
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? LastModifiedAtUtc { get; set; }
    }
}
