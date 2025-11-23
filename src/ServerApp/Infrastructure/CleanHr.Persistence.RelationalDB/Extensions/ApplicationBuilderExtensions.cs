using System.Threading.Tasks;
using CleanHr.Domain.Aggregates.DepartmentAggregate;
using CleanHr.Domain.Aggregates.EmployeeAggregate;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using CleanHr.Persistence.RelationalDB.SeedData;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Persistence.RelationalDB.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void ApplyDatabaseMigrations(this WebApplication app)
    {
        app.ThrowIfNull(nameof(app));

        using IServiceScope serviceScope = app.Services.CreateScope();
        CleanHrDbContext dbContext = serviceScope.ServiceProvider.GetRequiredService<CleanHrDbContext>();

        // TODO: Comment out this if you have SQL server installed on your machine.
        dbContext.Database.Migrate();
    }

    public static async Task SeedDatabaseAsync(this WebApplication app)
    {
        app.ThrowIfNull(nameof(app));

        using IServiceScope serviceScope = app.Services.CreateScope();
        var serviceProvider = serviceScope.ServiceProvider;

        var dbContext = serviceProvider.GetRequiredService<CleanHrDbContext>();
        var departmentRepository = serviceProvider.GetRequiredService<IDepartmentRepository>();
        var employeeRepository = serviceProvider.GetRequiredService<IEmployeeRepository>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var logger = serviceProvider.GetRequiredService<ILogger<DatabaseSeeder>>();

        var seeder = new DatabaseSeeder(
            dbContext,
            departmentRepository,
            employeeRepository,
            userManager,
            roleManager,
            logger);

        await seeder.SeedAsync();
    }
}
