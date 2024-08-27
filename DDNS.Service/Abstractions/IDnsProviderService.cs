using EnsyNet.Core.Results;
using System.Net;

namespace DDNS.Services.Abstractions;

public interface IDnsProviderService
{
    Task<Result> UpdateDnsRecord(IPAddress ipAddress, CancellationToken ct);
}
