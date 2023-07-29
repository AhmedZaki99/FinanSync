using FinanSyncApi.Data;
using FinanSyncData;

namespace FinanSyncApi.Core;

public interface IUserEntityService<TEntity, TRequestDto, TResponseDto>
    where TEntity : EntityBase
    where TRequestDto : class
    where TResponseDto : EntityResponseDto
{

    #region Read

    /// <summary>
    /// Get all entities for the given user.
    /// </summary>
    /// <remarks>
    /// This method is not preferred to use in case of big data handling.
    /// </remarks>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of type <typeparamref name="TEntity"/> mapped to <typeparamref name="TResponseDto"/>.</returns>
    IAsyncEnumerable<TResponseDto> GetEntitiesAsync(string userId);

    /// <summary>
    /// Find entity by id.
    /// </summary>
    /// <param name="userId">Id of the entity owner.</param>
    /// <param name="id">Entity id to search for.</param>
    /// <returns>The found entity if any, mapped to a <typeparamref name="TResponseDto"/>.</returns>
    Task<TResponseDto?> FindEntityAsync(string userId, string id, CancellationToken cancellationToken = default);

    #endregion

    #region Create

    /// <summary>
    /// Create a new entity after validating data provided by <typeparamref name="TRequestDto"/> object.
    /// </summary>
    /// <param name="userId">Id of the entity owner.</param>
    /// <param name="dto">The object containing data to create the entity of.</param>
    /// <param name="validateDtoProperties">
    /// If <see langword="true"/>, also validate the DTO properties using their associated 
    /// <see cref="System.ComponentModel.DataAnnotations.ValidationAttribute"/> attributes.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TOutput}"/> wrapping the created entity data,
    /// and providing a dictionary of errors occurred in the process, if any.
    /// </returns>
    Task<Result<TResponseDto>> CreateEntityAsync(string userId, TRequestDto dto, bool validateDtoProperties = false, CancellationToken cancellationToken = default);

    #endregion

    #region Update

    /// <summary>
    /// Update the underlying entity with data provided by <typeparamref name="TRequestDto"/> object,
    /// after validating it.
    /// </summary>
    /// <param name="userId">Id of the entity owner.</param>
    /// <param name="id">The id of the entity to update.</param>
    /// <param name="dto">The object containing data to update the underlying entity.</param>
    /// <param name="validateDtoProperties">s
    /// If <see langword="true"/>, also validate the DTO properties using their associated 
    /// <see cref="System.ComponentModel.DataAnnotations.ValidationAttribute"/> attributes.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TOutput}"/> wrapping the updated entity data,
    /// and providing a dictionary of errors occurred in the process, if any.        
    /// </returns>
    Task<Result<TResponseDto>> UpdateEntityAsync(string userId, string id, TRequestDto dto, bool validateDtoProperties = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update the underlying entity with the callback provided,
    /// which is applied to a <typeparamref name="TRequestDto"/> object, and then validate the result.
    /// </summary>
    /// <param name="userId">Id of the entity owner.</param>
    /// <param name="id">The id of the entity to update.</param>
    /// <param name="updateCallback">
    /// A callback used to update the underlying entity,
    /// which provides a <typeparamref name="TRequestDto"/> object to make necessary changes,
    /// and returns a <see cref="bool"/> to state whether the changes were made successfully.
    /// </param>
    /// <param name="validateDtoProperties">
    /// If <see langword="true"/>, also validate the DTO properties using their associated 
    /// <see cref="System.ComponentModel.DataAnnotations.ValidationAttribute"/> attributes.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TOutput}"/> wrapping the updated entity data,
    /// and providing a dictionary of errors occurred in the process, if any.        
    /// </returns>
    Task<Result<TResponseDto>> UpdateEntityAsync(string userId, string id, Func<TRequestDto, bool> updateCallback, bool validateDtoProperties = false, CancellationToken cancellationToken = default);

    #endregion

    #region Delete

    /// <summary>
    /// Delete entity by id.
    /// </summary>
    /// <param name="userId">Id of the entity owner.</param>
    /// <param name="id">The id of the entity to delete.</param>
    /// <returns>A <see cref="DeleteResult"/>.</returns>
    Task<DeleteResult> DeleteEntityAsync(string userId, string id, CancellationToken cancellationToken = default);

    #endregion

    #region Validation

    /// <summary>
    /// Check the data given with a <typeparamref name="TRequestDto"/> for any validation errors violating constraints of a <typeparamref name="TEntity"/> model.
    /// </summary>
    /// <param name="userId">Id of the entity owner.</param>
    /// <param name="dto">The input object to validate its data.</param>
    /// <returns>A dictionary with the set of validation errors, if any found.</returns>
    Task<Dictionary<string, string>?> ValidateRequestDataAsync(string userId, TRequestDto dto, TEntity? original = null, CancellationToken cancellationToken = default);

    #endregion

}
