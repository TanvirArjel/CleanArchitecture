using System;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Domain.Aggregates.IdentityAggregate;

public class ApplicationRole : IdentityRole<Guid>
{
}
