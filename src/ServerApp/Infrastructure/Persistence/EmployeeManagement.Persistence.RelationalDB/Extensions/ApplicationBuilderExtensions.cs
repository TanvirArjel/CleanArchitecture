using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TanvirArjel.ArgumentChecker;

namespace EmployeeManagement.Persistence.RelationalDB.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void ApplyDatabaseMigrations(this IApplicationBuilder app)
    {
        app.ThrowIfNull(nameof(app));

        using IServiceScope serviceScope = app.ApplicationServices.CreateScope();
        EmployeeManagementDbContext dbContext = serviceScope.ServiceProvider.GetRequiredService<EmployeeManagementDbContext>();

        dbContext.Database.Migrate();
    }
}
