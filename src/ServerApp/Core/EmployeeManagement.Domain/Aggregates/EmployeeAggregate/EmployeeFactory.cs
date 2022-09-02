using System;
using EmployeeManagement.Domain.Aggregates.DepartmentAggregate;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Domain.Aggregates.EmployeeAggregate;

[ScopedService]
public class EmployeeFactory
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeFactory(IDepartmentRepository departmentRepository, IEmployeeRepository employeeRepository)
    {
        _departmentRepository = departmentRepository;
        _employeeRepository = employeeRepository;
    }

    public Employee Create(
       string name,
       Guid departmentId,
       DateTime dateOfBirth,
       string email,
       string phoneNumber)
    {
        Employee employee = new Employee(_departmentRepository, _employeeRepository, name, departmentId, dateOfBirth, email, phoneNumber);

        return employee;
    }
}
