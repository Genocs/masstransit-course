using Genocs.MassTransit.Components.Consumers;
using Genocs.MassTransit.Components.CourierActivities;
using Genocs.MassTransit.Components.StateMachines;
using Genocs.MassTransit.Components.StateMachines.Activities;
using Genocs.MassTransit.Service;
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

        // This is a state machine Activity
        services.AddScoped<AcceptOrderActivity>();

        //services.AddScoped<RoutingSlipBatchEventConsumer>();

        services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
        services.AddMassTransit(cfg =>
        {
            // Consumer configuration
            cfg.AddConsumersFromNamespaceContaining<FulfillOrderConsumer>();

            // Routing slip configuration
            cfg.AddActivitiesFromNamespaceContaining<AllocateInventoryActivity>();

            cfg.AddSagaStateMachine<OrderStateMachine, OrderState>(typeof(OrderStateMachineDefinition))
                .RedisRepository();

            cfg.UsingRabbitMq(ConfigureBus);

            // Request client configuration
            //cfg.AddRequestClient<AllocateInventory>();
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
    //configurator.UseMessageData(new MongoDbMessageDataRepository("mongodb://127.0.0.1", "attachments"));
    //configurator.UseMessageScheduler(new Uri("queue:quartz"));

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

    // This configuration will configure the Activity Definition
    configurator.ConfigureEndpoints(context);
}