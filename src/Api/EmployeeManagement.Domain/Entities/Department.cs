using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagement.Domain.Entities
{
    public class Department : BaseEntity
    {
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public string Description { get; set; }
    }
}
