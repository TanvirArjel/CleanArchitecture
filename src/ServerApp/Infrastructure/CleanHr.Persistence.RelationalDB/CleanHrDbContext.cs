using System;
using System.Threading;
using System.Threading.Tasks;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using CleanHr.Persistence.RelationalDB.EntityConfigurations.EmployeeAggregate;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CleanHr.Persistence.RelationalDB;

internal class CleanHrDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public CleanHrDbContext(DbContextOptions<CleanHrDbContext> options)
        : base(options)
    {
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ////ChangeTracker.ApplyValueGenerationOnUpdate();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        ////ChangeTracker.ApplyValueGenerationOnUpdate();
        return base.SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (modelBuilder == null)
        {
            throw new ArgumentNullException(nameof(modelBuilder));
        }

        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EmployeeConfiguration).Assembly);
        ////modelBuilder.ApplyBaseEntityConfiguration(); // This should be called after calling the derived entity configurations
    }
}
