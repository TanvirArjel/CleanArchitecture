using System;
using TanvirArjel.ArgumentChecker;

namespace EmployeeManagement.Domain.Entities
{
    public class Employee : BaseEntity
    {
        public Employee(
            string name,
            int departmentId,
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

        public int Id { get; private set; }

        public string Name { get; private set; }

        public int DepartmentId { get; private set; }

        public DateTime DateOfBirth { get; private set; }

        public string Email { get; private set; }

        public string PhoneNumber { get; private set; }

        public Department Department { get; private set; }

        public void SetName(string name)
        {
            name.ThrowIfNullOrEmpty(nameof(name));

            name.ThrowIfOutOfLength(2, 50, nameof(name));

            Name = name;
        }

        public void SetDeparment(int departmentId)
        {
            departmentId.ThrowIfZeroOrNegative(nameof(departmentId));
            DepartmentId = departmentId;
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
            email.ThrowIfNullOrEmpty(nameof(email));
            Email = email;
        }

        public void SetPhoneNumber(string phoneNumber)
        {
            phoneNumber.ThrowIfNullOrEmpty(nameof(phoneNumber));
            PhoneNumber = phoneNumber;
        }
    }
}
