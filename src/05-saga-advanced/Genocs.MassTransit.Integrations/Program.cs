using Genocs.MassTransit.Integrations.Service;
using Genocs.MassTransit.Integrations.Service.Consumers;
using MassTransit;
using Serilog;
using Serilog.Events;
using System.Reflection;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("MassTransit", LogEventLevel.Debug)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Debug)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Debug)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        OpenTelemetryInitializer.Initialize(hostContext, services);
        //TelemetryAndLogging.Initialize(hostContext, services);

        services.AddMassTransit(x =>
        {
            // Point 1.
            //x.AddServiceBusMessageScheduler();

            x.SetKebabCaseEndpointNameFormatter();

            var entryAssembly = Assembly.GetEntryAssembly();
            x.AddConsumer<SettlementSubmittedConsumer>();

            x.UsingAzureServiceBus((context, cfg) =>
            {

                cfg.Host(hostContext.Configuration.GetConnectionString("AzureServiceBus"));

                // Point 2. See Point 1.
                //cfg.UseServiceBusMessageScheduler();

                // Option 1: Subscribe to SettlementSubmitted directly from the topic, instead of configuring a queue
                //cfg.SubscriptionEndpoint<SettlementSubmitted>("settlement-submitted", e =>
                //{
                //    e.ConfigureConsumer<SettlementSubmittedConsumer>(context);
                //});

                // Option 2: Subscribe to SettlementSubmitted with manual subscription and topic
                cfg.SubscriptionEndpoint("settlement-submitted-to-integration", "settlement-topic", e =>
                {
                    e.ConfigureConsumer<SettlementSubmittedConsumer>(context);
                });

                // Configure Endpoint to send messages over the transport
                cfg.ConfigureEndpoints(context);
            });
        });


        services.AddHostedService<ConsoleHostedService>();
    })
    .ConfigureLogging((hostingContext, logging) =>
    {
        logging.AddSerilog(dispose: true);
        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
    })
    .Build();

await host.RunAsync();

Log.CloseAndFlush();

// await TelemetryAndLogging.FlushAndCloseAsync();