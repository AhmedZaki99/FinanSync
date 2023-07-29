namespace FinanSyncData;

public sealed class UserResponseDto : EntityResponseDto
{

    public string Email { get; set; } = null!;

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }

    public ICollection<SettingResponseDto> Settings { get; set; } = null!;

}
