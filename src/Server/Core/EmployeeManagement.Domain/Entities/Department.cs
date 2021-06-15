using TanvirArjel.ArgumentChecker;

namespace EmployeeManagement.Domain.Entities
{
    public class Department : BaseEntity
    {
        public Department(string name, string description)
        {
            SetName(name);
            SetDescription(description);
        }

        // This is needed for EF Core query mapping.
        private Department()
        {
        }

        public int Id { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public void SetName(string name)
        {
            name.ThrowIfNullOrEmpty(nameof(name));
            name.ThrowIfOutOfLength(2, 50, nameof(name));

            Name = name;
        }

        public void SetDescription(string description)
        {
            description.ThrowIfNullOrEmpty(nameof(description));
            description.ThrowIfOutOfLength(20, 100, nameof(description));

            Description = description;
        }
    }
}
