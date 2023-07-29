using System.Diagnostics.CodeAnalysis;

namespace FinanSyncData;

public sealed class ProblemResponseDto
{

    public required string Status { get; set; }

    public required string Message { get; set; }
    public required string ProblemCode { get; set; }


    public ProblemResponseDto()
    {

    }

    [SetsRequiredMembers]
    public ProblemResponseDto(string status, string message, string problemCode)
    {
        Status = status;
        Message = message;
        ProblemCode = problemCode;
    }

}
