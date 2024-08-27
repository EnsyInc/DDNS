using System.Xml;

namespace DDNS.Core.Configurations;

public sealed record MonitorConfig
{
    public const string CONFIGURATION_NAME = "Monitor";

    public string StartingDelay { get; init; } = "PT1S";
    public TimeSpan StartingDelayTime => XmlConvert.ToTimeSpan(StartingDelay);

    public string PollingInterval { get; init; } = "PT5M";
    public TimeSpan PollingIntervalTime => XmlConvert.ToTimeSpan(PollingInterval);

    public bool IsValid()
        => PollingIntervalTime > TimeSpan.Zero
        && StartingDelayTime > TimeSpan.Zero;
}
