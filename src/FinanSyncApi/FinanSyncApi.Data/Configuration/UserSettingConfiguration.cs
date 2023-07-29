using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanSyncApi.Data;

public sealed class UserSettingConfiguration : IEntityTypeConfiguration<UserSetting>
{
    public void Configure(EntityTypeBuilder<UserSetting> builder)
    {
        builder
            .HasOne<AppUser>()
            .WithMany(user => user.Settings)
            .HasForeignKey(us => us.AppUserId);

        builder
            .HasOne(us => us.Setting)
            .WithMany()
            .HasForeignKey(us => us.SettingId);
    }
}
