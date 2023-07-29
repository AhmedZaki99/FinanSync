namespace FinanSyncApi.Core;

public class Result
{

    #region Properties

    public IDictionary<string, string> Errors { get; set; }

    public OperationError ErrorType { get; set; }
    public bool IsSuccessful => ErrorType == OperationError.None && Errors.Count == 0;

    #endregion


    #region Constructors

    public Result()
    {
        Errors = new Dictionary<string, string>();
        ErrorType = OperationError.None;
    }

    public Result(OperationError errorType, string message = "") : this()
    {
        ErrorType = errorType;
        Errors.Add(errorType.ToString(), message);
    }

    public Result(IDictionary<string, string> errors, OperationError errorType)
    {
        Errors = errors;
        ErrorType = errorType;
    }

    #endregion

}

public sealed class Result<TOutput> : Result where TOutput : class
{

    public TOutput? Output { get; set; }


    public Result(TOutput? output)
    {
        Output = output;
    }

    public Result(OperationError errorType, string message = "") : base(errorType, message)
    { }

    public Result(IDictionary<string, string> errors, OperationError errorType) : base(errors, errorType)
    { }

}
