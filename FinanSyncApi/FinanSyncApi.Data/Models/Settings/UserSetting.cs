using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FinanSyncApi.Data;

[PrimaryKey(nameof(AppUserId), nameof(SettingId))]
[EntityTypeConfiguration(typeof(UserSettingConfiguration))]
public sealed class UserSetting
{

    [Required]
    public string AppUserId { get; set; } = null!;

    [Required]
    public string SettingId { get; set; } = null!;
    public Setting? Setting { get; set; }

    [Required]
    [MaxLength(256)]
    public string? Value { get; set; } = null!;

}
