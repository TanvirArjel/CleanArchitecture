using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagement.Application.Dtos.EmployeeDtos
{
    public class UpdateEmployeeDto
    {
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public int DepartmentId { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}
