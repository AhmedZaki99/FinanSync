using FinanSyncData;

namespace FinanSyncApi.Core;

public interface ISettingsService
{

    Task<SettingResponseDto[]> GetSettingsAsync(string userId, CancellationToken cancellationToken = default);
    Task<SettingResponseDto[]> GetDefinedSettingsAsync(string userId, CancellationToken cancellationToken = default);

    Task<Result<SettingResponseDto[]>> SetSettingsAsync(string userId, IEnumerable<SettingRequestDto> settings, CancellationToken cancellationToken = default);
    Task<Result<SettingResponseDto[]>> ResetSettingsAsync(string userId, CancellationToken cancellationToken = default);

}
