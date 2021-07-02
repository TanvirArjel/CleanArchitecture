using System;
using TanvirArjel.ArgumentChecker;

namespace EmployeeManagement.Domain.Entities
{
    public class Employee : BaseEntity
    {
        public Employee(
            string name,
            Guid departmentId,
            DateTime dateOfBirth,
            string email,
            string phoneNumber)
        {
            SetName(name);
            SetDeparment(departmentId);
            SetDateOfBirth(dateOfBirth);
            SetEmail(email);
            SetPhoneNumber(phoneNumber);
        }

        // This is needed for EF core query mapping.
        private Employee()
        {
        }

        public string Name { get; private set; }

        public Guid DepartmentId { get; private set; }

        public DateTime DateOfBirth { get; private set; }

        public string Email { get; private set; }

        public string PhoneNumber { get; private set; }

        public Department Department { get; private set; }

        public void SetName(string name)
        {
            Name = name.ThrowIfNullOrEmpty(nameof(name))
                       .ThrowIfOutOfLength(2, 50, nameof(name));
        }

        public void SetDeparment(Guid departmentId)
        {
            DepartmentId = departmentId.ThrowIfEmpty(nameof(departmentId));
        }

        public void SetDateOfBirth(DateTime dateOfBirth)
        {
            DateTime minDateOfBirth = DateTime.UtcNow.AddYears(-115);
            DateTime maxDateOfBirth = DateTime.UtcNow.AddYears(-15);

            // Validate the minimum age.
            dateOfBirth.ThrowIfOutOfRange(minDateOfBirth, maxDateOfBirth, nameof(dateOfBirth), "The minimum age has to be 15 years.");
            DateOfBirth = dateOfBirth;
        }

        public void SetEmail(string email)
        {
            Email = email.ThrowIfNullOrEmpty(nameof(email))
                         .ThrowIfNotValidEmail(nameof(email));
        }

        public void SetPhoneNumber(string phoneNumber)
        {
            PhoneNumber = phoneNumber.ThrowIfNullOrEmpty(nameof(phoneNumber));
        }
    }
}
