using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using FinanSyncApi.Data;
using FinanSyncData;

namespace FinanSyncApi.Core;

public sealed class UserService : IUserService
{


    #region Dependencies

    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    #endregion

    #region Constructor

    public UserService(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext= dbContext;
        _mapper= mapper;
    }

    #endregion


    #region Implementation

    /// <inheritdoc/>
    public Task<bool> UserExistsAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId, nameof(userId));

        return _dbContext.Users
            .AnyAsync(u => u.Id == userId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<UserResponseDto?> GetUserDataAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId, nameof(userId));

        return await _dbContext
            .Users
            .Include(u => u.Settings)
                .ThenInclude(s => s.Setting)
            .ProjectTo<UserResponseDto>(_mapper.ConfigurationProvider)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    #endregion

    #region Helper Methods



    #endregion

}
