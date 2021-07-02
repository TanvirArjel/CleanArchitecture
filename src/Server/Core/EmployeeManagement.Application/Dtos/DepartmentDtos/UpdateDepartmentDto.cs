using System;

namespace EmployeeManagement.Application.Dtos.DepartmentDtos
{
    public class UpdateDepartmentDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }
    }
}
