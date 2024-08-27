namespace DDNS.Core.Configurations;

public sealed record IpGetterConfig
{
    public const string CONFIGURATION_NAME = "IpGetter";

    public string Endpoint { get; init; } = string.Empty;

    public bool IsValid()
        => !string.IsNullOrWhiteSpace(Endpoint);
}
