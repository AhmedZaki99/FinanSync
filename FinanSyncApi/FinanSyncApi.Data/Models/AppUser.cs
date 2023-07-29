using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FinanSyncApi.Data;

public class AppUser : IdentityUser
{

    [MaxLength(64)]
    public string? FirstName { get; set; }

    [MaxLength(64)]
    public string? LastName { get; set; }


    public ICollection<UserSetting> Settings { get; set; } = null!;

}
