namespace CleanHr.Api;

public abstract class EmployeeBaseModel
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public Guid DepartmentId { get; set; }

    ////[MinAge(15, 0, 0, ErrorMessage = "The minimum age has to be 15 years.")]
    public DateTime DateOfBirth { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }
}
