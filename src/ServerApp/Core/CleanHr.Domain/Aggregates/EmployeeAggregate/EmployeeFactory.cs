using System;
using CleanHr.Domain.Aggregates.DepartmentAggregate;
using CleanHr.Domain.ValueObjects;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace CleanHr.Domain.Aggregates.EmployeeAggregate;

[ScopedService]
public sealed class EmployeeFactory(IDepartmentRepository departmentRepository, IEmployeeRepository employeeRepository)
{
    public Employee Create(
       EmployeeName name,
       Guid departmentId,
       DateOfBirth dateOfBirth,
       Email email,
       PhoneNumber phoneNumber)
    {
        Employee employee = new(departmentRepository, employeeRepository, name, departmentId, dateOfBirth, email, phoneNumber);

        return employee;
    }
}
