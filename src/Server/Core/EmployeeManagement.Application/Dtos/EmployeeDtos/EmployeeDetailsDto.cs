using System;

namespace EmployeeManagement.Application.Dtos.EmployeeDtos
{
    public class EmployeeDetailsDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? LastModifiedAtUtc { get; set; }
    }
}
