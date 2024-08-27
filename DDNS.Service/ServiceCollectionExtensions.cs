using DDNS.Core.Configurations;
using DDNS.Core.Models;
using DDNS.Services.Abstractions;
using DDNS.Services.Implementations;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DDNS.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDDnsServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddIpGetter(configuration)
            .AddDnsService(configuration);
    }

    private static IServiceCollection AddDnsService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DnsConfig>()
            .Bind(configuration.GetSection(DnsConfig.CONFIGURATION_NAME))
            .Validate(config => config.IsValid(), $"Invalid {DnsConfig.CONFIGURATION_NAME} configuration.")
            .ValidateOnStart();

        var config = configuration.GetSection(DnsConfig.CONFIGURATION_NAME).Get<DnsConfig>()!;

        return config.Provider switch
        {
            DnsProviders.None => throw new NotImplementedException(),
            DnsProviders.CloudFlare => services.AddCloudFlare(config.CloudFlare!),
            _ => throw new NotImplementedException(),
        };
    }

    private static IServiceCollection AddCloudFlare(this IServiceCollection services, CloudFlareConfig config)
    {
        services.AddHttpClient(nameof(CloudFlareService), http =>
        {
            http.BaseAddress = new Uri(config.Endpoint);
        });
        services.AddSingleton<IDnsProviderService, CloudFlareService>();
        return services;
    }

    private static IServiceCollection AddIpGetter(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<IpGetterConfig>()
            .Bind(configuration.GetSection(IpGetterConfig.CONFIGURATION_NAME))
            .Validate(config => config.IsValid(), $"Invalid {IpGetterConfig.CONFIGURATION_NAME} configuration.")
            .ValidateOnStart();

        var config = configuration.GetSection(IpGetterConfig.CONFIGURATION_NAME).Get<IpGetterConfig>()!;

        services.AddHttpClient(nameof(IfConfigService), http =>
        {
            http.BaseAddress = new Uri(config.Endpoint);
        });
        services.AddSingleton<IIpService, IfConfigService>();
        return services;
    }
}
