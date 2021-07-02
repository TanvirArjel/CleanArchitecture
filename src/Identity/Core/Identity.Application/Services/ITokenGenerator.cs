using Identity.Domain.Entities;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace Identity.Application.Services
{
    [SingletonService]
    public interface ITokenGenerator
    {
        string GenerateToken(ApplicationUser applicationUser);
    }
}
