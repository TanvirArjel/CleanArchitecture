using System;

namespace EmployeeManagement.Domain.Entities
{
    public class Employee : BaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int DepartmentId { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public Department Department { get; set; }
    }
}
