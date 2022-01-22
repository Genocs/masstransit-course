using Genocs.MassTransit.Components.Consumers;
using Genocs.Service6;
using MassTransit;
using MassTransit.Definition;
using MassTransit.RabbitMqTransport;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using Serilog.Events;

DependencyTrackingTelemetryModule _module;
TelemetryClient _telemetryClient;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

Microsoft.Extensions.Hosting.IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        _module = new DependencyTrackingTelemetryModule();
        _module.IncludeDiagnosticSourceActivities.Add("MassTransit");

        TelemetryConfiguration configuration = TelemetryConfiguration.CreateDefault();
        configuration.InstrumentationKey = "6b4c6c82-3250-4170-97d3-245ee1449278";
        configuration.TelemetryInitializers.Add(new HttpDependenciesParsingTelemetryInitializer());

        _telemetryClient = new TelemetryClient(configuration);

        _module.Initialize(configuration);


        services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
        services.AddMassTransit(cfg =>
        {
            cfg.AddConsumersFromNamespaceContaining<SubmitOrderConsumer>();

            cfg.UsingRabbitMq(ConfigureBus);

        });

        services.AddHostedService<MassTransitConsoleHostedService>();
    })
    .ConfigureLogging((hostingContext, logging) =>
    {
        logging.AddSerilog(dispose: true);
        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
    })
    .Build();

await host.RunAsync();

Log.CloseAndFlush();


static void ConfigureBus(IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator configurator)
{
    configurator.ConfigureEndpoints(context);
}