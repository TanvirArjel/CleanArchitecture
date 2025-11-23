using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanHr.Domain.Aggregates.DepartmentAggregate;
using CleanHr.Domain.Aggregates.EmployeeAggregate;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CleanHr.Persistence.RelationalDB.SeedData;

internal sealed class DatabaseSeeder(
    CleanHrDbContext dbContext,
    IDepartmentRepository departmentRepository,
    IEmployeeRepository employeeRepository,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    ILogger<DatabaseSeeder> logger)
{
    public async Task SeedAsync()
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            logger.LogInformation("Starting database seeding...");

            // Check if data already exists
            bool hasRoles = await dbContext.Set<ApplicationRole>().AnyAsync();
            bool hasUsers = await dbContext.Set<ApplicationUser>().AnyAsync();
            bool hasDepartments = await dbContext.Set<Department>().AnyAsync();
            bool hasEmployees = await dbContext.Set<Employee>().AnyAsync();

            if (hasRoles)
            {
                logger.LogInformation("Database already contains roles. Skipping role seeding.");
            }
            else
            {
                logger.LogInformation("Seeding roles...");
                await SeedRolesAsync();
            }

            if (hasUsers)
            {
                logger.LogInformation("Database already contains users. Skipping user seeding.");
            }
            else
            {
                logger.LogInformation("Seeding users...");
                await SeedUsersAsync();
            }

            if (hasDepartments)
            {
                logger.LogInformation("Database already contains departments. Skipping department seeding.");
            }
            else
            {
                logger.LogInformation("Seeding departments...");
                await SeedDepartmentsAsync();
            }

            if (hasEmployees)
            {
                logger.LogInformation("Database already contains employees. Skipping employee seeding.");
            }
            else
            {
                logger.LogInformation("Seeding employees...");
                await SeedEmployeesAsync();
            }

            await transaction.CommitAsync();
            logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogError(ex, "An error occurred while seeding the database. Transaction rolled back.");
            throw;
        }
    }

    private async Task SeedRolesAsync()
    {
        string[] roleNames = ["Admin", "Manager", "Employee", "HR"];

        foreach (string roleName in roleNames)
        {
            ApplicationRole role = new()
            {
                Name = roleName,
                NormalizedName = roleName.ToUpperInvariant()
            };

            IdentityResult result = await roleManager.CreateAsync(role);

            if (!result.Succeeded && logger.IsEnabled(LogLevel.Warning))
            {
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                logger.LogWarning("Failed to create role {RoleName}: {Errors}", roleName, errors);
            }
        }

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Seeded {Count} roles", roleNames.Length);
        }
    }

    private async Task SeedUsersAsync()
    {
        // Create Admin user
        ApplicationUser adminUser = new()
        {
            FullName = "System Administrator",
            UserName = "admin@cleanhr.com",
            Email = "admin@cleanhr.com",
            NormalizedUserName = "ADMIN@CLEANHR.COM",
            NormalizedEmail = "ADMIN@CLEANHR.COM",
            EmailConfirmed = true,
            PhoneNumber = "+1234567800",
            PhoneNumberConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        IdentityResult adminResult = await userManager.CreateAsync(adminUser, "Admin@123");

        if (adminResult.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Created admin user: {Email}", adminUser.Email);
            }
        }
        else if (logger.IsEnabled(LogLevel.Warning))
        {
            string errors = string.Join(", ", adminResult.Errors.Select(e => e.Description));
            logger.LogWarning("Failed to create admin user: {Errors}", errors);
        }

        // Create Manager user
        ApplicationUser managerUser = new()
        {
            FullName = "Sarah Johnson",
            UserName = "manager@cleanhr.com",
            Email = "manager@cleanhr.com",
            NormalizedUserName = "MANAGER@CLEANHR.COM",
            NormalizedEmail = "MANAGER@CLEANHR.COM",
            EmailConfirmed = true,
            PhoneNumber = "+1234567801",
            PhoneNumberConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        IdentityResult managerResult = await userManager.CreateAsync(managerUser, "Manager@123");

        if (managerResult.Succeeded)
        {
            await userManager.AddToRoleAsync(managerUser, "Manager");

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Created manager user: {Email}", managerUser.Email);
            }
        }
        else if (logger.IsEnabled(LogLevel.Warning))
        {
            string errors = string.Join(", ", managerResult.Errors.Select(e => e.Description));
            logger.LogWarning("Failed to create manager user: {Errors}", errors);
        }

        // Create HR user
        ApplicationUser hrUser = new()
        {
            FullName = "Emma Wilson",
            UserName = "hr@cleanhr.com",
            Email = "hr@cleanhr.com",
            NormalizedUserName = "HR@CLEANHR.COM",
            NormalizedEmail = "HR@CLEANHR.COM",
            EmailConfirmed = true,
            PhoneNumber = "+1234567802",
            PhoneNumberConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        IdentityResult hrResult = await userManager.CreateAsync(hrUser, "Hr@123");

        if (hrResult.Succeeded)
        {
            await userManager.AddToRoleAsync(hrUser, "HR");

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Created HR user: {Email}", hrUser.Email);
            }
        }
        else if (logger.IsEnabled(LogLevel.Warning))
        {
            string errors = string.Join(", ", hrResult.Errors.Select(e => e.Description));
            logger.LogWarning("Failed to create HR user: {Errors}", errors);
        }

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Seeded 3 identity users");
        }
    }

    private async Task SeedDepartmentsAsync()
    {
        List<Department> departments = new();

        var departmentData = new[]
        {
            ("IT Department", "Information Technology department responsible for managing company's technical infrastructure and software development."),
            ("Human Resources", "Human Resources department handling employee relations, recruitment, and organizational development."),
            ("Finance", "Finance department managing company's financial operations, budgeting, and accounting activities."),
            ("Sales & Marketing", "Sales and Marketing department driving revenue growth and promoting company products and services."),
            ("Operations", "Operations department ensuring smooth day-to-day business operations and process optimization.")
        };

        foreach (var (name, description) in departmentData)
        {
            var result = await Department.CreateAsync(departmentRepository, name, description);

            if (result.IsSuccess)
            {
                departments.Add(result.Value);
            }
            else if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError("Failed to create department '{DepartmentName}': {Errors}", name, result.Error);
            }
        }

        if (departments.Count > 0)
        {
            await dbContext.Set<Department>().AddRangeAsync(departments);
            await dbContext.SaveChangesAsync();

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Seeded {Count} departments", departments.Count);
            }
        }
        else
        {
            logger.LogWarning("No departments were seeded due to validation errors");
        }
    }

    private async Task SeedEmployeesAsync()
    {
        // Get departments
        var departments = await dbContext.Set<Department>().ToListAsync();
        var itDept = departments.First(d => d.Name == "IT Department");
        var hrDept = departments.First(d => d.Name == "Human Resources");
        var financeDept = departments.First(d => d.Name == "Finance");

        List<Employee> employees = new();

        var employeeData = new[]
        {
            ("John", "Doe", itDept.Id, new DateTime(1990, 5, 15), "john.doe@cleanhr.com", "+1234567890"),
            ("Jane", "Smith", hrDept.Id, new DateTime(1988, 8, 22), "jane.smith@cleanhr.com", "+1234567891"),
            ("Michael", "Johnson", itDept.Id, new DateTime(1992, 3, 10), "michael.johnson@cleanhr.com", "+1234567892"),
            ("Emily", "Davis", financeDept.Id, new DateTime(1995, 11, 8), "emily.davis@cleanhr.com", "+1234567893"),
            ("Robert", "Brown", financeDept.Id, new DateTime(1987, 7, 30), "robert.brown@cleanhr.com", "+1234567894")
        };

        foreach (var (firstName, lastName, departmentId, dateOfBirth, email, phoneNumber) in employeeData)
        {
            var result = await Employee.CreateAsync(
                departmentRepository,
                employeeRepository,
                firstName,
                lastName,
                departmentId,
                dateOfBirth,
                email,
                phoneNumber);

            if (result.IsSuccess)
            {
                employees.Add(result.Value);
            }
            else if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError("Failed to create employee '{FirstName} {LastName}': {Errors}", firstName, lastName, result.Error);
            }
        }

        if (employees.Count > 0)
        {
            await dbContext.Set<Employee>().AddRangeAsync(employees);
            await dbContext.SaveChangesAsync();

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Seeded {Count} employees", employees.Count);
            }
        }
        else
        {
            logger.LogWarning("No employees were seeded due to validation errors");
        }
    }
}
