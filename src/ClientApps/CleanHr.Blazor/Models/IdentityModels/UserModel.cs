namespace CleanHr.Blazor.Models.IdentityModels;

public class UserModel
{
    public int Id { get; set; }

    public string FullName { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public bool IsActive { get; set; }
}
