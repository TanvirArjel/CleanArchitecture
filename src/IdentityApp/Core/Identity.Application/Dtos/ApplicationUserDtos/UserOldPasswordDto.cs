using System;

namespace Identity.Application.Dtos.ApplicationUserDtos
{
    public class UserOldPasswordDto
    {
        public Guid UserId { get; set; }

        public string PasswordHash { get; set; }

        public DateTime SetAtUtc { get; set; }
    }
}
