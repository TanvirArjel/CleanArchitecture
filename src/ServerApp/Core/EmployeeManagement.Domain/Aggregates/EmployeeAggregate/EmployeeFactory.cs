using System;
using System.Threading.Tasks;
using EmployeeManagement.Domain.Aggregates.DepartmentAggregate;
using EmployeeManagement.Domain.Exceptions;
using TanvirArjel.ArgumentChecker;
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

    public async Task<Employee> CreateAsync(
       string name,
       Guid departmentId,
       DateTime dateOfBirth,
       string email,
       string phoneNumber)
    {
        name.ThrowIfNullOrEmpty(nameof(name));
        departmentId.ThrowIfEmpty(nameof(departmentId));
        phoneNumber.ThrowIfNullOrEmpty(nameof(phoneNumber));

        bool isDepartmentExistent = await _departmentRepository.ExistsAsync(d => d.Id == departmentId);

        if (isDepartmentExistent == false)
        {
            throw new EntityNotFoundException(typeof(Department), departmentId);
        }

        bool isEmployeeExistent = await _employeeRepository.ExistsAsync(d => d.Email == email);

        if (isEmployeeExistent)
        {
            throw new InvalidOperationException("An employee already exists with the provided email.");
        }

        bool isPhoneNumberExistent = await _employeeRepository.ExistsAsync(d => d.PhoneNumber == phoneNumber);

        if (isPhoneNumberExistent)
        {
            throw new InvalidOperationException("An employee already exists with the provided phone number.");
        }

        Employee employee = new Employee(name, departmentId, dateOfBirth, email, phoneNumber);

        return employee;
    }
}
