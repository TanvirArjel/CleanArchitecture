using System;
using System.Threading.Tasks;
using EmployeeManagement.Domain.Aggregates.DepartmentAggregate;
using EmployeeManagement.Domain.Exceptions;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Domain.Aggregates.EmployeeAggregate
{
    [ScopedService]
    public class EmployeeManipulator
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeManipulator(IDepartmentRepository departmentRepository, IEmployeeRepository employeeRepository)
        {
            _departmentRepository = departmentRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<Employee> SetDepartmentAsync(Employee employee, Guid departmentId)
        {
            employee.ThrowIfNull(nameof(employee));
            departmentId.ThrowIfEmpty(nameof(departmentId));

            if (employee.DepartmentId.Equals(departmentId))
            {
                return employee;
            }

            bool isDepartmentExistent = await _departmentRepository.ExistsAsync(d => d.Id == departmentId);

            if (isDepartmentExistent == false)
            {
                throw new EntityNotFoundException(typeof(Department), departmentId);
            }

            employee.SetDeparment(departmentId);

            return employee;
        }

        public async Task<Employee> SetEmailAsync(Employee employee, string email)
        {
            employee.ThrowIfNull(nameof(employee));
            email.ThrowIfNullOrEmpty(nameof(email));

            if (employee.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
            {
                return employee;
            }

            bool isPhoneNumberExistent = await _employeeRepository.ExistsAsync(d => d.Email == email);

            if (isPhoneNumberExistent)
            {
                throw new InvalidOperationException("An employee already exists with the provided email.");
            }

            employee.SetEmail(email);

            return employee;
        }

        public async Task<Employee> SetPhoneNumberAsync(Employee employee, string phoneNumber)
        {
            employee.ThrowIfNull(nameof(employee));
            phoneNumber.ThrowIfNullOrEmpty(nameof(phoneNumber));

            if (employee.PhoneNumber.Equals(phoneNumber, StringComparison.OrdinalIgnoreCase))
            {
                return employee;
            }

            bool isPhoneNumberExistent = await _employeeRepository.ExistsAsync(d => d.PhoneNumber == phoneNumber);

            if (isPhoneNumberExistent)
            {
                throw new InvalidOperationException("An employee already exists with the provided phone number.");
            }

            employee.SetPhoneNumber(phoneNumber);

            return employee;
        }
    }
}
