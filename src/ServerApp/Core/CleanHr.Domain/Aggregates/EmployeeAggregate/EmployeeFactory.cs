using System;
using CleanHr.Domain.Aggregates.DepartmentAggregate;
using CleanHr.Domain.ValueObjects;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace CleanHr.Domain.Aggregates.EmployeeAggregate;

[ScopedService]
public sealed class EmployeeFactory
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeFactory(IDepartmentRepository departmentRepository, IEmployeeRepository employeeRepository)
    {
        _departmentRepository = departmentRepository;
        _employeeRepository = employeeRepository;
    }

    public Employee Create(
       Name name,
       Guid departmentId,
       DateOfBirth dateOfBirth,
       Email email,
       PhoneNumber phoneNumber)
    {
        Employee employee = new Employee(_departmentRepository, _employeeRepository, name, departmentId, dateOfBirth, email, phoneNumber);

        return employee;
    }
}
