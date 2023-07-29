using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using FinanSyncApi.Data;
using FinanSyncData;
using System.ComponentModel.DataAnnotations;

namespace FinanSyncApi.Core;

public abstract class UserEntityService<TEntity, TRequestDto, TResponseDto> : IUserEntityService<TEntity, TRequestDto, TResponseDto>
    where TEntity : UserEntity
    where TRequestDto : class
    where TResponseDto : EntityResponseDto
{


    #region Protected Dependencies

    protected ApplicationDbContext AppDbContext { get; }
    protected DbSet<TEntity> EntityDbSet { get; }

    protected IMapper Mapper { get; }

    #endregion

    #region Constructor

    public UserEntityService(ApplicationDbContext dbContext, DbSet<TEntity> dbSet, IMapper mapper)
    {
        AppDbContext= dbContext;
        EntityDbSet= dbSet;
        Mapper= mapper;
    }

    #endregion


    #region Read

    /// <inheritdoc/>
    public IAsyncEnumerable<TResponseDto> GetEntitiesAsync(string userId)
    {
        ArgumentNullException.ThrowIfNull(userId, nameof(userId));

        return BasicQuery()
            .Where(e => e.AppUserId == userId)
            .ProjectTo<TResponseDto>(Mapper.ConfigurationProvider)
            .AsNoTracking()
            .AsAsyncEnumerable();
    }

    /// <inheritdoc/>
    public virtual Task<TResponseDto?> FindEntityAsync(string userId, string id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId, nameof(userId));
        ArgumentNullException.ThrowIfNull(id, nameof(id));

        return BasicQuery()
            .Where(e => e.AppUserId == userId)
            .ProjectTo<TResponseDto>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    #endregion

    #region Create

    /// <inheritdoc/>
    public virtual async Task<Result<TResponseDto>> CreateEntityAsync(string userId, TRequestDto dto, bool validateDtoProperties = false, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId, nameof(userId));
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        var errors = validateDtoProperties ? ValidateObject(dto) : null;
        if (errors is not null)
        {
            return new(errors, OperationError.ValidationError);
        }

        errors = await ValidateRequestDataAsync(userId, dto, cancellationToken: cancellationToken);
        if (errors is not null)
        {
            return new(errors, OperationError.ValidationError);
        }

        var entity = MapForCreate(dto, userId);

        EntityDbSet.Add(entity);

        errors = await TrySaveChangesAsync(cancellationToken);
        if (errors is not null)
        {
            return new(errors, OperationError.DatabaseError);
        }
        return new(Mapper.Map<TResponseDto>(entity));
    }

    #endregion

    #region Update

    /// <inheritdoc/>
    public virtual Task<Result<TResponseDto>> UpdateEntityAsync(string userId, string id, TRequestDto dto, bool validateDtoProperties = false, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId, nameof(userId));
        ArgumentNullException.ThrowIfNull(id, nameof(id));
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        var errors = validateDtoProperties ? ValidateObject(dto) : null;
        if (errors is not null)
        {
            return Task.FromResult(new Result<TResponseDto>(errors, OperationError.ValidationError));
        }

        return UpdateEntityAsync(userId, id, updateDto =>
        {
            updateDto = dto;
            return true;
        }, cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<Result<TResponseDto>> UpdateEntityAsync(string userId, string id, Func<TRequestDto, bool> updateCallback, bool validateDtoProperties = false, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId, nameof(userId));
        ArgumentNullException.ThrowIfNull(id, nameof(id));
        ArgumentNullException.ThrowIfNull(updateCallback, nameof(updateCallback));

        var entity = await BasicQuery().FirstOrDefaultAsync(e => e.AppUserId == userId && e.Id == id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return new(OperationError.EntityNotFound);
        }
        var dto = Mapper.Map<TRequestDto>(entity);

        if (!updateCallback.Invoke(dto))
        {
            return new(OperationError.ExternalError);
        }

        var errors = validateDtoProperties ? ValidateObject(dto) : null;
        if (errors is not null)
        {
            return new(errors, OperationError.ValidationError);
        }

        errors = await ValidateRequestDataAsync(userId, dto, entity, cancellationToken: cancellationToken);
        if (errors is not null)
        {
            return new(errors, OperationError.ValidationError);
        }

        entity = MapForUpdate(dto, entity);

        var entry = AppDbContext.Entry(entity);
        if (entry.State == EntityState.Unchanged)
        {
            entry.State = EntityState.Modified;
        }

        await AppDbContext.SaveChangesAsync(cancellationToken);

        return new(Mapper.Map<TResponseDto>(entity));
    }

    #endregion

    #region Delete

    /// <inheritdoc/>
    public virtual async Task<DeleteResult> DeleteEntityAsync(string userId, string id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userId, nameof(userId));
        ArgumentNullException.ThrowIfNull(id, nameof(id));

        var entity = await CascadeQuery()
            .FirstOrDefaultAsync(e => e.AppUserId == userId && e.Id == id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return DeleteResult.EntityNotFound;
        }
        EntityDbSet.Remove(entity);

        return await AppDbContext.SaveChangesAsync(cancellationToken) > 0 ? DeleteResult.Success : DeleteResult.Failed;
    }

    #endregion

    #region Abstract Methods

    /// <inheritdoc/>
    public abstract Task<Dictionary<string, string>?> ValidateRequestDataAsync(string userId, TRequestDto dto, TEntity? original = null, CancellationToken cancellationToken = default);

    #endregion


    #region Mapping

    /// <summary>
    /// Maps the given Dto to an entity instance for data creation.
    /// </summary>
    /// <remarks>
    /// Could be overridden to provide further adjustment on mapped element.
    /// </remarks>
    /// <param name="dto">The object to map.</param>
    /// <param name="userId">Id of the entity owner.</param>
    /// <returns>The mapped instance of the entity.</returns>
    protected virtual TEntity MapForCreate(TRequestDto dto, string userId)
    {
        var entity = Mapper.Map<TEntity>(dto);
        entity.AppUserId = userId;

        return entity;
    }

    /// <summary>
    /// Maps the given Dto into the original state of the entity to apply changes.
    /// </summary>
    /// <remarks>
    /// Could be overridden to provide further adjustment on mapped element.
    /// </remarks>
    /// <param name="dto">The object to map.</param>
    /// <param name="original">The original state of the entity.</param>
    /// <returns>The mapped instance of the entity.</returns>
    protected virtual TEntity MapForUpdate(TRequestDto dto, TEntity original)
    {
        return Mapper.Map(dto, original);
    }

    #endregion

    #region Navigation

    /// <summary>
    /// Provides the base query for CRUD operations.
    /// </summary>
    /// <remarks>
    /// Could be overridden to include basic level query operations.
    /// </remarks>
    protected virtual IQueryable<TEntity> BasicQuery()
    {
        return EntityDbSet;
    }
    
    /// <summary>
    /// Provides a query including navigation required for cascade delete operation.
    /// </summary>
    /// <remarks>
    /// Should be overridden to include dependent items required before deletion, if any.
    /// </remarks>
    protected virtual IQueryable<TEntity> CascadeQuery()
    {
        return EntityDbSet;
    }

    #endregion


    #region Helper Methods

    protected static Dictionary<string, string> OneErrorDictionary(string key, string message) => new()
    {
        [key] = message
    };

    private async Task<Dictionary<string, string>?> TrySaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (await AppDbContext.SaveChangesAsync(cancellationToken) > 0)
        {
            return null;
        }
        return OneErrorDictionary("Server Error", "Failed to save data.");
    }

    private static Dictionary<string, string>? ValidateObject<T>(T objectToValidate) where T : class
    {
        List<ValidationResult> results = new();
        ValidationContext context = new(objectToValidate);

        Validator.TryValidateObject(objectToValidate, context, results, true);

        if (results.Count > 0)
        {
            var pairs = results.Select(e => new KeyValuePair<string, string>(e.MemberNames.First(), e.ErrorMessage ?? "Invalid value."));
            return new(pairs);
        }
        return null;
    }

    #endregion


}
