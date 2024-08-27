using DDNS.Services.Abstractions;
using DDNS.Services.Models;
using EnsyNet.Core.Results;

using Microsoft.Extensions.Logging;

using System.Net;

namespace DDNS.Services.Implementations;

internal sealed class IfConfigService : IIpService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<IfConfigService> _logger;

    private const string IpEndpoint = "/ip";

    public IfConfigService(
        IHttpClientFactory httpClientFactory,
        ILogger<IfConfigService> logger)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<IPAddress>> GetIpAddressOfMachine(CancellationToken ct)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, IpEndpoint);

        var httpClient = _httpClientFactory.CreateClient(nameof(IfConfigService));

        using var result = await httpClient.SendAsync(request, ct);
        if (!result.IsSuccessStatusCode)
        {
            var error = await result.Content.ReadAsStringAsync(CancellationToken.None);
            _logger.LogError("Failed to get ip address ifconfig.me. Status code: {statusCode}. Error: {error}", result.StatusCode, error);
            return Result.FromError<IPAddress>(new FailedToGetIpAddressError(error));
        }

        var rawIpAddress = await result.Content.ReadAsStringAsync(ct);
        var ipAddress = IPAddress.Parse(rawIpAddress);

        return Result.Ok(ipAddress);
    }
}
