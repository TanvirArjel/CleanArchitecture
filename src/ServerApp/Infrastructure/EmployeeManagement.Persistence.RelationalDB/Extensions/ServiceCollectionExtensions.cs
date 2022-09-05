using System;
using EmployeeManagement.Domain.Aggregates.DepartmentAggregate;
using EmployeeManagement.Domain.Aggregates.EmployeeAggregate;
using EmployeeManagement.Domain.Aggregates.IdentityAggregate;
using EmployeeManagement.Persistence.RelationalDB.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TanvirArjel.EFCore.GenericRepository;

namespace EmployeeManagement.Persistence.RelationalDB.Extensions;

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

        services.AddDbContext<EmployeeManagementDbContext>(options =>
        {
            options.UseLoggerFactory(MyLoggerFactory);
            options.EnableSensitiveDataLogging(true);
            options.UseSqlServer(connectionString, builder =>
            {
                ////builder.EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), null);
                builder.MigrationsAssembly("EmployeeManagement.Persistence.RelationalDB");
                builder.MigrationsHistoryTable("__EFCoreMigrationsHistory", schema: "_Migration");
            });
        });

        services.AddIdentity<User, ApplicationRole>(options =>
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
        .AddEntityFrameworkStores<EmployeeManagementDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        services.AddGenericRepository<EmployeeManagementDbContext>();
        services.AddQueryRepository<EmployeeManagementDbContext>();
    }
}
