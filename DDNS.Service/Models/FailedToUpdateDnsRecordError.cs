using DDNS.Core.Utils;

using EnsyNet.Core.Results;

namespace DDNS.Services.Models;

public sealed record FailedToUpdateDnsRecordError : Error
{
    public FailedToUpdateDnsRecordError(string error) : base(Constants.ErrorCodes.FAILED_TO_UPDATE_DNS_RECORD_ERROR, error) { }
}
