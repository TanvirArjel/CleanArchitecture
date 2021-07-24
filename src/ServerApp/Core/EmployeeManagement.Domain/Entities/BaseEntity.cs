using System;

namespace EmployeeManagement.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? LastModifiedAtUtc { get; set; }
    }
}
