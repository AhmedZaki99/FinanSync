using System.Diagnostics.CodeAnalysis;

namespace FinanSyncData;

public sealed class SuccessResponseDto
{

    public required string Status { get; set; }

    public required string Message { get; set; }


    public SuccessResponseDto()
    {

    }

    [SetsRequiredMembers]
    public SuccessResponseDto(string status, string message)
    {
        Status = status;
        Message = message;
    }

}
