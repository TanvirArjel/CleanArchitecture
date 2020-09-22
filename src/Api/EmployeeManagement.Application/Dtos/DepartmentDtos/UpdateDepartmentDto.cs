namespace EmployeeManagement.Application.Dtos.DepartmentDtos
{
    public class UpdateDepartmentDto
    {
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }
    }
}
