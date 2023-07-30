namespace FinanSyncApi;

public sealed class JwtBearerOptions
{

    public const string Key = "Bearer";


    public string TokenSecret { get; set; } = string.Empty;
    public int TokenExpirationMinutes { get; set; } = -1;

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;

}
