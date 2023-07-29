using System.ComponentModel.DataAnnotations;

namespace FinanSyncApi.Data;

public abstract class UserEntity : EntityBase
{

    [Required]
    public string AppUserId { get; set; } = null!;

}
