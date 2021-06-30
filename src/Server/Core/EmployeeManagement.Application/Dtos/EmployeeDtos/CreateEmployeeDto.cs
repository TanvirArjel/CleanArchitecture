using System;

namespace EmployeeManagement.Application.Dtos.EmployeeDtos
{
    public class CreateEmployeeDto
    {
        public string Name { get; set; }

        public int DepartmentId { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}
