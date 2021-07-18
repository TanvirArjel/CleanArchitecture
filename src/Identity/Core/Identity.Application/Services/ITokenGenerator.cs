using System.Collections.Generic;
using Identity.Domain.Entities;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace Identity.Application.Services
{
    [SingletonService]
    public interface ITokenGenerator
    {
        string GenerateJwtToken(ApplicationUser applicationUser, IEnumerable<string> roles);

        string GenerateRefreshToken();
    }
}
