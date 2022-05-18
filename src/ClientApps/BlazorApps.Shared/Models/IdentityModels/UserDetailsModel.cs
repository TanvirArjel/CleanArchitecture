using System.Collections.Generic;

namespace BlazorApps.Shared.Models.IdentityModels;

public class UserDetailsModel
{
    public int Id { get; set; }

    public string FullName { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public bool IsActive { get; set; }

    public ICollection<RoleModel> Roles { get; set; }
}
