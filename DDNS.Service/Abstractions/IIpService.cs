using EnsyNet.Core.Results;

using System.Net;

namespace DDNS.Services.Abstractions;

public interface IIpService
{
    Task<Result<IPAddress>> GetIpAddressOfMachine(CancellationToken ct);
}
