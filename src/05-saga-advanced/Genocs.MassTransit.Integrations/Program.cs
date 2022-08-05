using Genocs.MassTransit.Integrations.Service;
using Serilog;
using Serilog.Events;



Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {

        TelemetryAndLogging.Initialize("InstrumentationKey=f28b8a8c-bf65-44a6-9976-e56613fef466;IngestionEndpoint=https://westeurope-5.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/");


        services.AddHostedService<ConsoleHostedService>();
    })
    .ConfigureLogging((hostingContext, logging) =>
    {
        logging.AddSerilog(dispose: true);
        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
    })
    .Build();

await host.RunAsync();

await TelemetryAndLogging.FlushAndCloseAsync();

Log.CloseAndFlush();
