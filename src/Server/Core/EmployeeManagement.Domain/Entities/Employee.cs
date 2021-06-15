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

            if (name.Length < 2 && name.Length > 50)
            {
                throw new ArgumentException("The length of the name must be in between 2 and 50 characters.");
            }

            Name = name;
        }

        public void SetDeparment(int departmentId)
        {
            departmentId.ThrowIfZeroOrNegative(nameof(departmentId));
            DepartmentId = departmentId;
        }

        public void SetDateOfBirth(DateTime dateOfBirth)
        {
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
