using System;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.Infrastructure.Services
{
    public static class InfrastructureServicesConfigurtionExtensions
    {
        public static void AddInfrasturctureConifugrations(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // Infrastructure services configuration will go here.
        }
    }
}
