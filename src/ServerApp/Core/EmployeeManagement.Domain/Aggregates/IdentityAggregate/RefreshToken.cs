using System;

namespace EmployeeManagement.Domain.Aggregates.IdentityAggregate;

public class RefreshToken
{
    public Guid UserId { get; set; }

    public string Token { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime ExpireAtUtc { get; set; }

    // Navigation properties
    public ApplicationUser ApplicationUser { get; set; }
}
