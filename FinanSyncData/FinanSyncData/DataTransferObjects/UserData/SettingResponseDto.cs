namespace FinanSyncData;

public sealed class SettingResponseDto
{

    public string Name { get; set; } = null!;
    public TypeCode ClrTypeCode { get; set; } 

    public string? Value { get; set; }
    public string? DefaultValue { get; set; }

}
