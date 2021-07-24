using System;
using System.Threading;
using System.Threading.Tasks;
using EmployeeManagement.Persistence.RelationalDB.EntityConfigurations.DomainEntities;
using EmployeeManagement.Persistence.RelationalDB.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Persistence.RelationalDB
{
    internal class EmployeeManagementDbContext : DbContext
    {
        public EmployeeManagementDbContext(DbContextOptions<EmployeeManagementDbContext> options)
            : base(options)
        {
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            ChangeTracker.ApplyValueGenerationOnUpdate();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            ChangeTracker.ApplyValueGenerationOnUpdate();
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
            modelBuilder.ApplyBaseEntityConfiguration(); // This should be called after calling the derived entity configurations
        }
    }
}
