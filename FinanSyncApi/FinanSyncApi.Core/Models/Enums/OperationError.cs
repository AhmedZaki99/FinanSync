namespace FinanSyncApi.Core;

public enum OperationError
{
    None = 0,
    EntityNotFound = 1,
    ValidationError = 2,
    DatabaseError = 3,
    ExternalError = 4
}
