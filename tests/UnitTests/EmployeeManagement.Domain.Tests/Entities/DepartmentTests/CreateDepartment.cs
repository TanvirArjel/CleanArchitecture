using EmployeeManagement.Domain.Aggregates.DepartmentAggregate;
using System;
using Xunit;

namespace UnitTests.EmployeeManagement.Domain.Tests.Entities.DepartmentTests
{
    public class CreateDepartment
    {
        private Department _department;
        private string _name = "Software";
        private string _description = "Internal software department";

        public CreateDepartment()
        {
            _department = new Department(_name, _description);
        }

        [Fact]
        public void Should_Throws_ArgumentException_For_EmptyName()
        {
            string newName = "";
            Assert.Throws<ArgumentException>(() => _department.SetName(newName));
        }

        [Fact]
        public void Should_Throws_ArgumentException_For_EmptyDescription()
        {
            string newDescription = "";
            Assert.Throws<ArgumentException>(() => _department.SetDescription(newDescription));
        }

        [Fact]
        public void Should_Throws_OutOfRangeException_When_NameLangthLessThan2()
        {
            string newName = "s";
            Assert.Throws<ArgumentOutOfRangeException>(() => _department.SetName(newName));
        }

        [Fact]
        public void Should_Throws_OutOfRangeException_When_NameLangthGreaterThan50()
        {
            string newName = "Lorem Ipsum is simply dummy text of the printing and typesetting industry";
            Assert.Throws<ArgumentOutOfRangeException>(() => _department.SetName(newName));
        }

        [Fact]
        public void Should_Throws_OutOfRangeException_When_DescriptionLangthLessThan20()
        {
            string newDescription = "s";
            Assert.Throws<ArgumentOutOfRangeException>(() => _department.SetName(newDescription));
        }

        [Fact]
        public void Should_Throws_OutOfRangeException_When_DescriptionLangthGreaterThan100()
        {
            string newDescription = "Lorem Ipsum is simply dummy text of the printing and typesetting " +
                "industry. Lorem Ipsum has been the test department";
            Assert.Throws<ArgumentOutOfRangeException>(() => _department.SetName(newDescription));
        }
    }
}
