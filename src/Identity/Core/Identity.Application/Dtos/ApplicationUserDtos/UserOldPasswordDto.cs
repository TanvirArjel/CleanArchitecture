using System;

namespace Identity.Application.Dtos.ApplicationUserDtos
{
    public class UserOldPasswordDto
    {
        public Guid UserId { get; set; }

        public string Password { get; set; }

        public DateTime SetAtUtc { get; set; }
    }
}
