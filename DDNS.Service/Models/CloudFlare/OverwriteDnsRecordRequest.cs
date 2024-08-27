namespace DDNS.Services.Models.CloudFlare;

public sealed record OverwriteDnsRecordRequest
{
    public required string Content { get; init; }
    public required string Name { get; init; }
    public bool Proxied { get; init; }
    public required string Type { get; init; }
    public string Comment { get; init; } = string.Empty;
    public required string Id { get; init; }
    public IEnumerable<string> Tags { get; init; } = [];
    public int Ttl { get; init; } = 1;
}
