using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinanSyncApi.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{

    #region Entity Sets

    public DbSet<Setting> Settings => Set<Setting>();
    public DbSet<UserSetting> UserSettings => Set<UserSetting>();

    #endregion


    #region Constructors

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    #endregion


    #region Fluent API

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


    }

    #endregion

    #region Convention

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<decimal>()
            .HavePrecision(19, 4);
    }

    #endregion

}
