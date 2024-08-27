using DDNS.Core.Utils;

using EnsyNet.Core.Results;

namespace DDNS.Services.Models;

public sealed record FailedToGetIpAddressError: Error
{
    public FailedToGetIpAddressError(string error) : base(Constants.ErrorCodes.FAILED_TO_GET_IP_ADDRESS_ERROR, error) { }
}
