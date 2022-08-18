using Azure.Monitor.OpenTelemetry.Exporter;
using Genocs.MassTransit.Components.Consumers;
using Genocs.MassTransit.Components.CourierActivities;
using Genocs.MassTransit.Components.HttpClients;
using Genocs.MassTransit.Components.StateMachines;
using Genocs.MassTransit.Components.StateMachines.Activities;
using Genocs.MassTransit.Service;
using Genocs.MassTransit.Warehouse.Contracts;
using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using System.Diagnostics;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("MassTransit", LogEventLevel.Debug)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Debug)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Debug)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();


Microsoft.Extensions.Hosting.IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddOpenTelemetryTracing(builder =>
        {
            builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService("IssuerService")
                    .AddTelemetrySdk()
                    .AddEnvironmentVariableDetector())
                .AddSource("MassTransit")
                .AddMongoDBInstrumentation()
                .AddAzureMonitorTraceExporter(o =>
                {
                    o.ConnectionString = hostContext.Configuration["ApplicationInsightsConnectionString"];
                })
                .AddJaegerExporter(o =>
                {
                    o.AgentHost = HostMetadataCache.IsRunningInContainer ? "jaeger" : "localhost";
                    o.AgentPort = 6831;
                    o.MaxPayloadSizeInBytes = 4096;
                    o.ExportProcessorType = ExportProcessorType.Batch;
                    o.BatchExportProcessorOptions = new BatchExportProcessorOptions<Activity>
                    {
                        MaxQueueSize = 2048,
                        ScheduledDelayMilliseconds = 5000,
                        ExporterTimeoutMilliseconds = 30000,
                        MaxExportBatchSize = 512,
                    };
                });
        });

        // This is a state machine Activity
        services.AddScoped<OrderRequestedActivity>();

        //services.AddScoped<RoutingSlipBatchEventConsumer>();

        services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
        services.AddMassTransit(cfg =>
        {
            // Consumer configuration
            cfg.AddConsumersFromNamespaceContaining<FulfillOrderConsumer>();

            // Routing slip configuration
            cfg.AddActivitiesFromNamespaceContaining<AllocateInventoryActivity>();

            // Saga handling Order state
            cfg.AddSagaStateMachine<OrderStateMachine, OrderState>(typeof(OrderStateMachineDefinition))
                .RedisRepository();

            // Saga handling Payment state
            cfg.AddSagaStateMachine<PaymentStateStateMachine, PaymentState>(typeof(PaymentStateStateMachineDefinition))
                .RedisRepository();

            cfg.UsingRabbitMq(ConfigureBus);

            // Request client configuration
            //{ KebabCaseEndpointNameFormatter.Instance.Consumer<AllocateInventory>()}
            cfg.AddRequestClient<AllocateInventory>(new Uri($"exchange:Genocs.MassTransit.Warehouse.Contracts:AllocateInventory"));

        });

        services.AddHostedService<MassTransitConsoleHostedService>();

        // Add services to the container.
        services.AddHttpClient<CustomerClient>();

    })
    .ConfigureLogging((hostingContext, logging) =>
    {
        logging.AddSerilog(dispose: true);
        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
    })
    .Build();

await host.RunAsync();

//await TelemetryAndLogging.FlushAndCloseAsync();

Log.CloseAndFlush();


static void ConfigureBus(IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator configurator)
{
    //configurator.UseMessageData(new MongoDbMessageDataRepository("mongodb://127.0.0.1", "attachments"));

    //configurator.ReceiveEndpoint(KebabCaseEndpointNameFormatter.Instance.Consumer<RoutingSlipBatchEventConsumer>(), e =>
    //{
    //    e.PrefetchCount = 20;

    //    e.Batch<RoutingSlipCompleted>(b =>
    //    {
    //        b.MessageLimit = 10;
    //        b.TimeLimit = TimeSpan.FromSeconds(5);

    //        b.Consumer<RoutingSlipBatchEventConsumer, RoutingSlipCompleted>(context);
    //    });
    //});

    // This configuration allow to handle the Scheduling
    configurator.UseMessageScheduler(new Uri("queue:quartz"));

    // This configuration will configure the Activity Definition
    configurator.ConfigureEndpoints(context);
}
