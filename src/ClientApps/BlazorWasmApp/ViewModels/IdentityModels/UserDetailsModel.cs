using System.Collections.Generic;

namespace BlazorWasmApp.ViewModels.IdentityModels
{
    public class UserDetailsModel
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }

        public List<RoleModel> Roles { get; set; }
    }
}
