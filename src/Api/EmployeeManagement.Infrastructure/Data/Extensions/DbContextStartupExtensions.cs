using System;
using System.Collections.Generic;
using System.Linq;
using EmployeeManagement.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Infrastructure.Data.Extensions
{
    public static class DbContextStartupExtensions
    {
        public static readonly ILoggerFactory MyLoggerFactory
            = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter((category, level) =>
                        category == DbLoggerCategory.Database.Command.Name
                        && level == LogLevel.Information)
                    .AddConsole();
            });

        public static void AddEmployeeManagementDbContext(
            this IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment hostingEnvironment)
        {
            string connectionName = hostingEnvironment.IsDevelopment() ? "EmployeeDbConnection" : "EmployeeDbConnection";
            string connectionString = configuration.GetConnectionString(connectionName);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string is either null or empty.");
            }

            services.AddDbContext<EmployeeManagementDbContext>(options =>
            {
                options.UseLoggerFactory(MyLoggerFactory);
                options.EnableSensitiveDataLogging(true);
                options.UseSqlServer(connectionString, builder =>
                {
                    ////builder.EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), null);
                    builder.MigrationsAssembly("EmployeeManagement.Infrastructure");
                    builder.MigrationsHistoryTable("__EFCoreMigrationsHistory", schema: "_Migration");
                });
            });
        }

        public static void ApplyValueGenerationOnUpdate(this ChangeTracker changeTracker)
        {
            if (changeTracker == null)
            {
                throw new ArgumentNullException(nameof(changeTracker));
            }

            List<EntityEntry<BaseEntity>> modifiedEntries = changeTracker.Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Modified).ToList();

            if (modifiedEntries.Any())
            {
                foreach (EntityEntry<BaseEntity> entityEntry in modifiedEntries)
                {
                    bool hasAnyPropertyValueModified = entityEntry.Properties.Any(p => p.IsModified);

                    if (hasAnyPropertyValueModified)
                    {
                        bool isMenuallySet = entityEntry.Property(p => p.LastModifiedAtUtc).IsModified;
                        if (isMenuallySet)
                        {
                            throw new ApplicationException("You can't set the value for LastModifiedAtUtc property. The value of this property will be set by system.");
                        }

                        entityEntry.Property(p => p.LastModifiedAtUtc).CurrentValue = DateTime.UtcNow;
                    }
                }
            }
        }
    }
}
