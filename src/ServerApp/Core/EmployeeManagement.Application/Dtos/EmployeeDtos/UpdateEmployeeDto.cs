using System;

namespace EmployeeManagement.Application.Dtos.EmployeeDtos
{
    public class UpdateEmployeeDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid DepartmentId { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}
