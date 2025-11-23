using System;
using CleanHr.Domain.Aggregates.DepartmentAggregate;
using CleanHr.Domain.Aggregates.EmployeeAggregate;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using CleanHr.Persistence.RelationalDB.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Persistence.RelationalDB.Extensions;

public static class ServiceCollectionExtensions
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

    public static void AddRelationalDbContext(
        this IServiceCollection services,
        string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string is either null or empty.");
        }

        services.AddDbContext<CleanHrDbContext>(options =>
        {
            options.UseLoggerFactory(MyLoggerFactory);
            options.EnableSensitiveDataLogging(true);
            options.UseSqlServer(connectionString, builder =>
            {
                ////builder.EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), null);
                builder.MigrationsAssembly("CleanHr.Persistence.RelationalDB");
                builder.MigrationsHistoryTable("__EFCoreMigrationsHistory", schema: "_Migration");
            });
        });

        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            // Password settings.
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;

            // Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings.
            options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.@";
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<CleanHrDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();

        services.AddGenericRepository<CleanHrDbContext>();
        services.AddQueryRepository<CleanHrDbContext>();
    }
}
