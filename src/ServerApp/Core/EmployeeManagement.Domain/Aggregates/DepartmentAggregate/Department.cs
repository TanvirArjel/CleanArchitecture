using EmployeeManagement.Domain.Entities;
using TanvirArjel.ArgumentChecker;

namespace EmployeeManagement.Domain.Aggregates.DepartmentAggregate
{
    public class Department : BaseEntity
    {
        internal Department(string name, string description)
        {
            SetName(name);
            SetDescription(description);
        }

        // This is needed for EF Core query mapping or deserialization.
        private Department()
        {
        }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public void SetDescription(string description)
        {
            Description = description.ThrowIfNullOrEmpty(nameof(description))
                                     .ThrowIfOutOfLength(20, 100, nameof(description));
        }

        internal void SetName(string name)
        {
            Name = name.ThrowIfNullOrEmpty(nameof(name))
                       .ThrowIfOutOfLength(2, 50, nameof(name));
        }
    }
}
