using Microsoft.Extensions.DependencyInjection;

namespace FinanSyncApi.Core;

/// <summary>
/// A builder for configuring Core Services.
/// </summary>
public sealed class CoreBuilder
{

    /// <summary>
    /// Gets the <see cref="IServiceCollection"/> where core services are configured.
    /// </summary>
    public IServiceCollection Services { get; }


    public CoreBuilder(IServiceCollection services)
    {
        Services = services;
    }

}
