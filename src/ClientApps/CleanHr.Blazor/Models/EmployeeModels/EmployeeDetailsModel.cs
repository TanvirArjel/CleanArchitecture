using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CleanHr.Blazor.Models.EmployeeModels;

public class EmployeeDetailsModel
{
    public Guid Id { get; set; }

    [DisplayName("Employee Name")]
    public string Name { get; set; }

    public Guid DepartmentId { get; set; }

    [DisplayName("Department Name")]
    public string DepartmentName { get; set; }

    [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
    [DisplayName("Date Of Birth")]
    public DateTime DateOfBirth { get; set; }

    public string Email { get; set; }

    [DisplayName("Phone Number")]
    public string PhoneNumber { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? LastModifiedAtUtc { get; set; }
}
