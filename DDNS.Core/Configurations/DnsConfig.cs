using DDNS.Core.Models;

namespace DDNS.Core.Configurations;

public sealed record DnsConfig
{
    public const string CONFIGURATION_NAME = "DDNS";

    public DnsProviders Provider { get; init; } = DnsProviders.None;
    public CloudFlareConfig? CloudFlare { get; init; }
    public string SubDomainName { get; init; } = string.Empty;

    public bool IsValid()
        => !string.IsNullOrWhiteSpace(SubDomainName)
        && Provider switch
        {
            DnsProviders.None => false,
            DnsProviders.CloudFlare => CloudFlare != null && CloudFlare.IsValid(),
            _ => throw new NotImplementedException(),
        };
}

public sealed record CloudFlareConfig
{
    public string ApiKey { get; init; } = string.Empty;
    public string Endpoint { get; init; } = string.Empty;
    public string DnsRecordId { get; init; } = string.Empty;
    public string ZoneId { get; init; } = string.Empty;

    public bool IsValid()
        => !string.IsNullOrWhiteSpace(ApiKey)
        && !string.IsNullOrWhiteSpace(Endpoint)
        && !string.IsNullOrWhiteSpace(DnsRecordId)
        && !string.IsNullOrWhiteSpace(ZoneId);
}
