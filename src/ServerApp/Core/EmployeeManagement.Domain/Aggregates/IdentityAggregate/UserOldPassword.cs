using System;

namespace EmployeeManagement.Domain.Aggregates.IdentityAggregate;

public class UserOldPassword
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string PasswordHash { get; set; }

    public DateTime SetAtUtc { get; set; }

    public User User { get; set; }
}
