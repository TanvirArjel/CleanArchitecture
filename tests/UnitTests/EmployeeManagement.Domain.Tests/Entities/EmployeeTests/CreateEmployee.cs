using EmployeeManagement.Domain.Aggregates.DepartmentAggregate;
using EmployeeManagement.Domain.Aggregates.EmployeeAggregate;
using System;
using Xunit;

namespace UnitTests.EmployeeManagement.Domain.Tests.Entities.DepartmentTests
{
    public class CreateEmployee
    {
        private Department _department;
        private string _name = "Software";
        private string _description = "Internal software department";
        private Employee _employee;
        private string _employeeName = "Abdullah";
        private DateTime _dateOfBirth = DateTime.Now.AddYears(-20);
        private string _phone = "0122269947";
        private string _email = "abd@gmail.com";

        public CreateEmployee()
        {
            _department = new Department(_name, _description);
            _employee = new Employee(_employeeName, _department.Id, _dateOfBirth, _email, _phone);
        }

        [Fact]
        public void Should_Throws_ArgumentException_For_EmptyName()
        {
            string newName = "";
            Assert.Throws<ArgumentException>(() => _employee.SetName(newName));
        }

        [Fact]
        public void Should_Throws_OutOfRangeException_When_NameLangthLessThan2()
        {
            string newName = "s";
            Assert.Throws<ArgumentOutOfRangeException>(() => _employee.SetName(newName));
        }

        [Fact]
        public void Should_Throws_OutOfRangeException_When_NameLangthGreaterThan50()
        {
            string newName = "Lorem Ipsum is simply dummy text of the printing and typesetting industry";
            Assert.Throws<ArgumentOutOfRangeException>(() => _employee.SetName(newName));
        }

        [Fact]
        public void Should_Throws_ArgumentException_When_DepartmentIsNull()
        {
            Guid departmentId = Guid.Empty;
            Assert.Throws<ArgumentException>(() => _employee.SetDeparment(departmentId));
        }

        [Fact]
        public void Should_Throws_OutOfRangeException_When_AgeLessThan15Years()
        {
            DateTime dateofBirth = DateTime.UtcNow.AddYears(-10);
            Assert.Throws<ArgumentOutOfRangeException>(() => _employee.SetDateOfBirth(dateofBirth));
        }

        [Fact]
        public void Should_Throws_ArgumentException_For_EmptyEmail()
        {
            string newEmail = "";
            Assert.Throws<ArgumentException>(() => _employee.SetEmail(newEmail));
        }

        [Fact]
        public void Should_Throws_ArgumentException_When_InvalidEmailFormat()
        {
            string newEmail = "abca";
            Assert.Throws<ArgumentException>(() => _employee.SetEmail(newEmail));
        }

        [Fact]
        public void Should_Throws_ArgumentException_For_EmptyPhoneNumber()
        {
            string newPhoneNumber = "";
            Assert.Throws<ArgumentException>(() => _employee.SetPhoneNumber(newPhoneNumber));
        }
    }
}
