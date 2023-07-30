using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using FinanSyncApi.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FinanSyncApi;

public sealed class TokenAuthService : ITokenAuthService
{


    #region Dependencies

    private readonly JwtBearerOptions _authOptions;

    #endregion

    #region Constructor

    public TokenAuthService(IOptionsSnapshot<JwtBearerOptions> optionsAccessor)
    {
        _authOptions= optionsAccessor.Value;
    }

    #endregion


    #region Implementation

    public BearerToken GenerateToken(AppUser user)
    {
        DateTime? expirationTime = _authOptions.TokenExpirationMinutes < 0
            ? null
            : DateTime.UtcNow.AddMinutes(_authOptions.TokenExpirationMinutes);

        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!)
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authOptions.TokenSecret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            issuer: _authOptions.Issuer,
            audience: _authOptions.Audience,
            expires: expirationTime,
            claims: claims,
            signingCredentials: credentials);

        return new()
        {
            Value = new JwtSecurityTokenHandler().WriteToken(token),
            ExpirationDate = expirationTime
        };
    }

    #endregion

}
