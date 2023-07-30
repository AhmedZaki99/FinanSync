using FinanSyncApi.Data;

namespace FinanSyncApi;

public interface ITokenAuthService
{

    BearerToken GenerateToken(AppUser user);

}
