using DDNS.Core.Configurations;
using DDNS.Core.Utils;
using DDNS.Services.Abstractions;
using DDNS.Services.Models;
using DDNS.Services.Models.CloudFlare;

using EnsyNet.Core.Results;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mime;
using System.Text;

namespace DDNS.Services.Implementations;

internal sealed class CloudFlareService : IDnsProviderService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly DnsConfig _config;
    private readonly CloudFlareConfig _cloudFlareConfig;
    private readonly ILogger<CloudFlareService> _logger;

    private const string _recordEndpoint = "/client/v4/zones/{0}/dns_records/{1}";

    public CloudFlareService(
        IHttpClientFactory httpClientFactory,
        IOptions<DnsConfig> config,
        ILogger<CloudFlareService> logger)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _config = config.Value ?? throw new ArgumentNullException(nameof(config));
        _cloudFlareConfig = config.Value.CloudFlare ?? throw new ArgumentNullException(nameof(config));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result> UpdateDnsRecord(IPAddress ipAddress, CancellationToken ct)
    {
        var body = new OverwriteDnsRecordRequest
        {
            Content = ipAddress.ToString(),
            Name = _config.SubDomainName,
            Proxied = true,
            Type = "A",
            Id = _cloudFlareConfig.DnsRecordId,
        };

        var route = string.Format(_recordEndpoint, _cloudFlareConfig.ZoneId, _cloudFlareConfig.DnsRecordId);
        using var request = new HttpRequestMessage(HttpMethod.Put, route);
        request.Headers.Authorization = new ("Bearer", _cloudFlareConfig.ApiKey);
        request.Content = new StringContent(JsonSerializerHelper.Serialize(body), Encoding.UTF8, MediaTypeNames.Application.Json);

        var httpClient = _httpClientFactory.CreateClient(nameof(CloudFlareService));

        using var result = await httpClient.SendAsync(request, ct);
        if (!result.IsSuccessStatusCode)
        {
            var error = await result.Content.ReadAsStringAsync(CancellationToken.None);
            _logger.LogError("Failed to update DNS record. Status code: {statusCode}. Error: {error}", result.StatusCode, error);
            return Result.FromError(new FailedToUpdateDnsRecordError(error));
        }

        return Result.Ok();
    }
}
