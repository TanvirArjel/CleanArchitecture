using System;
using System.Threading.Tasks;
using CleanHr.Domain.Aggregates.DepartmentAggregate;
using CleanHr.Domain.Primitives;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace CleanHr.Domain.Aggregates.EmployeeAggregate;

[ScopedService]
public sealed class EmployeeFactory(IDepartmentRepository departmentRepository, IEmployeeRepository employeeRepository)
{
    public Task<Result<Employee>> CreateAsync(
       string firstName,
       string lastName,
       Guid departmentId,
       DateTime dateOfBirth,
       string email,
       string phoneNumber)
    {
        return Employee.CreateAsync(departmentRepository, employeeRepository, firstName, lastName, departmentId, dateOfBirth, email, phoneNumber);
    }
}
