using DDNS;
using DDNS.Core.Configurations;
using DDNS.Services;

using NLog;
using NLog.Web;

#pragma warning disable CA1031 // Do not catch general exception types
try
{
    var hostBuilder = Host.CreateDefaultBuilder(args);
    hostBuilder.ConfigureServices(Configure)
        .ConfigureAppConfiguration((context, configBuilder) =>
        {
            configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
            configBuilder.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: false);
            configBuilder.AddEnvironmentVariables();
        }).ConfigureLogging((context, loggingBuilder) =>
        {
            loggingBuilder.ClearProviders();
        }).UseNLog();
        LogManager.Setup().LoadConfigurationFromFile("nlog.config");

    var app = hostBuilder.Build();
    app.Run();
}
catch (Exception ex)
{
    LogManager.GetCurrentClassLogger().Fatal(ex, "An error occurred in the application");
    LogManager.Shutdown();
    Environment.Exit(1);
}
finally
{
    LogManager.Shutdown();
}
#pragma warning restore CA1031 // Do not catch general exception types

static void Configure(HostBuilderContext context, IServiceCollection services)
{
    services.AddDDnsServices(context.Configuration);

    services.AddOptions<MonitorConfig>()
            .Bind(context.Configuration.GetSection(MonitorConfig.CONFIGURATION_NAME))
            .Validate(config => config.IsValid(), $"Invalid {MonitorConfig.CONFIGURATION_NAME} configuration.")
            .ValidateOnStart();
    services.AddHostedService<DDnsUpdaterBackgroundService>();
}
