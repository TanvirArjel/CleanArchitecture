using System;

namespace Identity.Domain.Entities;

public class RefreshToken
{
    public Guid UserId { get; set; }

    public string Token { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime ExpireAtUtc { get; set; }

    public User ApplicationUser { get; set; }
}
