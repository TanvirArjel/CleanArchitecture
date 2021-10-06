using System;

namespace EmployeeManagement.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAtUtc { get; protected set; }

        public DateTime? LastModifiedAtUtc { get; protected set; }
    }
}
