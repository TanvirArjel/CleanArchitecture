using System;

namespace Identity.Domain.Entities
{
    public class PasswordResetCode
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string Code { get; set; }

        public DateTime SentAtUtc { get; set; }

        public DateTime? UsedAtUtc { get; set; }
    }
}
