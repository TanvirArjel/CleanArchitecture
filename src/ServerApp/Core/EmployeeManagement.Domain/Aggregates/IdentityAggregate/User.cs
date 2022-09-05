using System;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Domain.Aggregates.IdentityAggregate;

public class User : IdentityUser<Guid>
{
    public string FullName { get; set; }

    public string DialCode { get; set; }

    public string LanguageCulture { get; set; }

    public bool IsDisabled { get; set; }

    public DateTime? LastLoggedInAtUtc { get; set; }

    public RefreshToken RefreshToken { get; set; }
}
