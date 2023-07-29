namespace FinanSyncData;

public sealed class AuthenticationResponseDto
{

    public required string BearerToken { get; set; }

    public DateTime? TokenExpirationTime { get; set; } 

}
