using System;
using Microsoft.AspNetCore.Identity;

namespace CleanHr.Domain.Aggregates.IdentityAggregate;

public class ApplicationRole : IdentityRole<Guid>
{
}
