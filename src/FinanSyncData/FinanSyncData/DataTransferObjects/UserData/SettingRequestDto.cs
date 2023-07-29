using System.ComponentModel.DataAnnotations;

namespace FinanSyncData;

public sealed class SettingRequestDto
{

    [Required]
    [StringLength(50, ErrorMessage = "{0} should not be more than {1} characters.")]
    public string Name { get; set; } = null!; 

    [Required]
    [StringLength(200, ErrorMessage = "{0} should not be more than {1} characters.")]
    public string Value { get; set; } = null!;

}