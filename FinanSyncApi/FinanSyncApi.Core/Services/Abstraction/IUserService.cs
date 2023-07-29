using FinanSyncData;

namespace FinanSyncApi.Core;

public interface IUserService
{

    Task<bool> UserExistsAsync(string userId, CancellationToken cancellationToken = default);

    Task<UserResponseDto?> GetUserDataAsync(string userId, CancellationToken cancellationToken = default);

}
