using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Persistence.RelationalDB.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void ApplyDatabaseMigrations(this WebApplication app)
    {
        app.ThrowIfNull(nameof(app));

        using IServiceScope serviceScope = app.Services.CreateScope();
        CleanHrDbContext dbContext = serviceScope.ServiceProvider.GetRequiredService<CleanHrDbContext>();

        dbContext.Database.Migrate();
    }
}
