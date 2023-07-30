using FinanSyncApi.Core;
using FinanSyncApi.Data;
using FinanSyncApi.JsonConverters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FinanSyncApi;

public class Program
{

    #region Application Entry

    public static void Main(string[] args)
    {
        // Initialize the application builder.
        var builder = WebApplication.CreateBuilder(args);

        // TODO: Handle Server-Side errors properly for production environment.
        //       See https://learn.microsoft.com/en-us/aspnet/core/web-api/handle-errors

        // Add services to the container.
        ConfigureControllers(builder);
        ConfigureIdentity(builder);
        ConfigureAuthentication(builder);
        ConfigureCoreServices(builder);
        ConfigureSwagger(builder);


        // Build the web application.
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        ConfigurePipeline(app);


        // Run the application.
        app.Run();
    }

    #endregion

    #region Services Configuration

    private static void ConfigureControllers(WebApplicationBuilder builder)
    {
        // Add Controllers.
        builder.Services
            .AddControllers(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            })
            .AddNewtonsoftJson(options =>
                options.SerializerSettings.Converters.Add(new EnumJsonConverter()));
    }

    private static void ConfigureIdentity(WebApplicationBuilder builder)
    {
        // Add Identity.
        builder.Services
            .AddIdentityCore<AppUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireUppercase = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
    }

    private static void ConfigureAuthentication(WebApplicationBuilder builder)
    {
        // Configure Jwt Authentication.
        var authConfig = builder.Configuration.GetSection($"Authentication:Schemes:{JwtBearerOptions.Key}");
        var authOptions = authConfig.Get<JwtBearerOptions>()
            ?? throw new InvalidOperationException("Couldn't resolve authentication options from configuration providers.");
        builder.Services.Configure<JwtBearerOptions>(authConfig);

        // Add authentication.
        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = authOptions.Issuer,
                    ValidAudience = authOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.TokenSecret))
                };
            });
    }

    private static void ConfigureCoreServices(WebApplicationBuilder builder)
    {
        // Add core services.
        builder.Services
            .AddCoreServices()
            .AddSqlServerDb(builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Couldn't resolve default database connection string from configuration providers."));

        // Add application services.
        builder.Services.AddScoped<ITokenAuthService, TokenAuthService>();
    }

    private static void ConfigureSwagger(WebApplicationBuilder builder)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }

    #endregion

    #region Pipeline Configuration

    private static void ConfigurePipeline(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.MapControllers()
            .RequireAuthorization();
    }

    #endregion
}