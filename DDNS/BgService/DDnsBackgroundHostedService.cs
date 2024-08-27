using DDNS.Core.Configurations;
using DDNS.Services.Abstractions;

using Microsoft.Extensions.Options;

namespace DDNS.BgService;

internal sealed class DDnsBackgroundHostedService : BackgroundService
{
    private readonly IIpService _ipService;
    private readonly IDnsProviderService _dnsProviderService;
    private readonly MonitorConfig _config;
    private readonly ILogger<DDnsBackgroundHostedService> _logger;

    private string? _lastIp;

    public DDnsBackgroundHostedService(
        IIpService ipService,
        IDnsProviderService dnsProviderService,
        IOptions<MonitorConfig> config,
        ILogger<DDnsBackgroundHostedService> logger)
    {
        _ipService = ipService ?? throw new ArgumentNullException(nameof(ipService));
        _dnsProviderService = dnsProviderService ?? throw new ArgumentNullException(nameof(dnsProviderService));
        _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _lastIp = null;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await Task.Delay(_config.StartingDelayTime, ct);
        _logger.LogInformation("Starting DDNS background service.");

        while (!ct.IsCancellationRequested)
        {
            await UpdateDns(ct);
            await Task.Delay(_config.PollingIntervalTime, ct);
        }

        _logger.LogInformation("Stopping DDNS background service.");
    }

    private async Task UpdateDns(CancellationToken ct)
    {
        _logger.LogInformation("Getting IP Address of current machine.");
        var ipResult = await _ipService.GetIpAddressOfMachine(ct);
        if (ipResult.HasError)
        {
            _logger.LogError("Failed to get IP Address of current machine. Error: {error}", ipResult.Error);
            return;
        }
        var ip = ipResult.Data!;
        _logger.LogInformation("Retrieved IP: {ip}.", ip);
        if (_lastIp == ip.ToString())
        {
            _logger.LogInformation("IP Address has not changed. Skipping DNS record update.");
            return;
        }

        _logger.LogInformation("Detected IP Address change. Updating DNS record...");
        var updateDnsRecordResult = await _dnsProviderService.UpdateDnsRecord(ip, ct);
        if (updateDnsRecordResult.HasError)
        {
            _logger.LogError("Failed to update DNS record. Error: {error}", ipResult.Error);
            return;
        }
        _lastIp = ip.ToString();
        _logger.LogInformation("Updated DNS Record to IP: {ip}.", ip);
    }
}
