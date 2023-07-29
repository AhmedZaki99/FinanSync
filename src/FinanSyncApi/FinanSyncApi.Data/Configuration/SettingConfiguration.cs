using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanSyncApi.Data;

public sealed class SettingConfiguration : IEntityTypeConfiguration<Setting>
{
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder
            .Property(s => s.ClrTypeCode)
            .HasDefaultValue(TypeCode.String);
    }
}
