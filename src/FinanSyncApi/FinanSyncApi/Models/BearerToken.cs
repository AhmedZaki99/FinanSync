using System.Diagnostics.CodeAnalysis;

namespace FinanSyncApi;

public class BearerToken
{

    #region Properties

    public required string Value { get; init; }

    public DateTime? ExpirationDate { get; init; }

    #endregion


    #region Constructors

    public BearerToken()
    {

    }

    [SetsRequiredMembers]
    public BearerToken(string value, DateTime expirationDate) : this()
    {
        Value = value;
        ExpirationDate = expirationDate;
    }

    #endregion


}
