using System;
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

        public int Id { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public void SetName(string name)
        {
            name.ThrowIfNullOrEmpty(nameof(name));

            if (name.Length < 2 && name.Length > 50)
            {
                throw new ArgumentException("The length of the name must be in between 2 and 50 characters.");
            }

            Name = name;
        }

        public void SetDescription(string description)
        {
            description.ThrowIfNullOrEmpty(nameof(description));

            if (description.Length < 20 && description.Length > 100)
            {
                throw new ArgumentException("The length of the description must be in between 20 and 100 characters.");
            }

            Description = description;
        }
    }
}
