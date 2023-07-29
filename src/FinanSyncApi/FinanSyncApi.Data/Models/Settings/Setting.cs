using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FinanSyncApi.Data;

[Index(nameof(Name), IsUnique = true)]
[EntityTypeConfiguration(typeof(SettingConfiguration))]
public sealed class Setting : EntityBase
{

    [Required]
    [MaxLength(64)]
    public string Name { get; set; } = null!;

    [Required]
    public TypeCode ClrTypeCode { get; set; }

    [MaxLength(128)]
    public string? DefaultValue { get; set; }


    #region Read Only Properties

    public Type ClrType => ClrTypeCode switch
    {
        TypeCode.Boolean => typeof(bool),
        TypeCode.Char => typeof(char),
        TypeCode.Byte => typeof(byte),
        TypeCode.Int16 => typeof(short),
        TypeCode.Int32 => typeof(int),
        TypeCode.Int64 => typeof(long),
        TypeCode.Single => typeof(float),
        TypeCode.Double => typeof(double),
        TypeCode.Decimal => typeof(decimal),
        TypeCode.DateTime => typeof(DateTime),
        TypeCode.String => typeof(string),
        _ => typeof(string)
    };

    #endregion

}
