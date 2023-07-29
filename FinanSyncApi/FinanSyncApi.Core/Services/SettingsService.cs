using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using FinanSyncApi.Data;
using FinanSyncData;
using System.ComponentModel;

namespace FinanSyncApi.Core;

public sealed class SettingsService : ISettingsService
{


    #region Dependencies

    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    #endregion

    #region Constructor

    public SettingsService(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext= dbContext;
        _mapper = mapper;
    }

    #endregion


    #region Implementation

    #region Read

    /// <inheritdoc/>
    public async Task<SettingResponseDto[]> GetSettingsAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId, nameof(userId));

        var user = await _dbContext
            .Users
            .Include(u => u.Settings)
                .ThenInclude(s => s.Setting)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken: cancellationToken)
            ?? throw new ArgumentException("Invalid user id", nameof(userId));

        var defaultSettings = await _dbContext
            .Settings
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);

        var undefinedSettings = defaultSettings
            .ExceptBy(user.Settings.Select(s => s.SettingId), s => s.Id);

        foreach (var setting in undefinedSettings)
        {
            user.Settings.Add(new()
            {
                AppUserId = userId,
                SettingId = setting.Id,
                Setting = setting,
                Value = setting.DefaultValue ?? string.Empty
            });
        }
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SettingResponseDto[]>(user.Settings);
    }

    /// <inheritdoc/>
    public Task<SettingResponseDto[]> GetDefinedSettingsAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId, nameof(userId));

        return _dbContext
            .UserSettings
            .Include(s => s.Setting)
            .Where(s => s.AppUserId == userId)
            .ProjectTo<SettingResponseDto>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);
    }

    #endregion

    #region Write

    /// <inheritdoc/>
    public async Task<Result<SettingResponseDto[]>> SetSettingsAsync(string userId,
                                                                              IEnumerable<SettingRequestDto> settings,
                                                                              CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId, nameof(userId));
        ArgumentNullException.ThrowIfNull(settings, nameof(settings));

        var user = await _dbContext
            .Users
            .Include(u => u.Settings)
                .ThenInclude(s => s.Setting)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken: cancellationToken)
            ?? throw new ArgumentException("Invalid user id", nameof(userId));

        var defaultSettings = await _dbContext
            .Settings
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);

        var errors = new Dictionary<string, string>();
        foreach (var setting in settings)
        {
            // Ignore non existing settings.
            var defaultSetting = defaultSettings.FirstOrDefault(s => s.Name == setting.Name);
            if (defaultSetting is null)
            {
                continue;
            }

            // Validate Type.
            if (!IsValidType(setting.Value, defaultSetting.ClrType))
            {
                errors.Add(setting.Name!, "Invalid value type.");
            }
            if (errors.Count > 0)
            {
                continue;
            }

            var userSetting = user.Settings.FirstOrDefault(s => s.Setting!.Name == setting.Name);
            if (userSetting is not null)
            {
                userSetting.Value = setting.Value;
                continue;
            }
            user.Settings.Add(new()
            {
                AppUserId = userId,
                SettingId = defaultSetting.Id,
                Setting = defaultSetting,
                Value = setting.Value
            });
        }
        await _dbContext.SaveChangesAsync(cancellationToken);

        return errors.Count > 0
            ? new(errors, OperationError.ValidationError)
            : new(_mapper.Map<SettingResponseDto[]>(user.Settings));
    }

    /// <inheritdoc/>
    public async Task<Result<SettingResponseDto[]>> ResetSettingsAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId, nameof(userId));

        var userSettings = await _dbContext
            .UserSettings
            .Include(s => s.Setting)
            .Where(s => s.AppUserId == userId)
            .ToArrayAsync(cancellationToken);

        foreach (var userSetting in userSettings)
        {
            userSetting.Value = userSetting.Setting!.DefaultValue ?? string.Empty;
        }
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new(await GetDefinedSettingsAsync(userId, cancellationToken));
    }

    #endregion

    #endregion

    #region Helper Methods

    private static bool IsValidType(string? value, Type type)
    {
        var converter = TypeDescriptor.GetConverter(type);
        return value is not null && converter.IsValid(value);
    }

    #endregion

}
