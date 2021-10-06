using System;
using System.Threading.Tasks;
using EmployeeManagement.Domain.Aggregates.DepartmentAggregate;
using EmployeeManagement.Domain.Exceptions;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Domain.Aggregates.EmployeeAggregate
{
    [ScopedService]
    public class EmployeeManipulator
    {
        private readonly IRepository _repository;

        public EmployeeManipulator(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<Employee> SetDepartmentAsync(Employee employee, Guid departmentId)
        {
            employee.ThrowIfNull(nameof(employee));
            departmentId.ThrowIfEmpty(nameof(departmentId));

            if (employee.DepartmentId.Equals(departmentId))
            {
                return employee;
            }

            bool isDepartmentExistent = await _repository.ExistsAsync<Department>(d => d.Id == departmentId);

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

            if (employee.Email.Equals(email))
            {
                return employee;
            }

            bool isPhoneNumberExistent = await _repository.ExistsAsync<Employee>(d => d.Email == email);

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

            if (employee.PhoneNumber.Equals(phoneNumber))
            {
                return employee;
            }

            bool isPhoneNumberExistent = await _repository.ExistsAsync<Employee>(d => d.PhoneNumber == phoneNumber);

            if (isPhoneNumberExistent)
            {
                throw new InvalidOperationException("An employee already exists with the provided phone number.");
            }

            employee.SetPhoneNumber(phoneNumber);

            return employee;
        }
    }
}
