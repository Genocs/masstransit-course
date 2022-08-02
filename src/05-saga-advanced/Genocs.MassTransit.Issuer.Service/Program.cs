using Genocs.MassTransit.Components.Consumers;
using Genocs.MassTransit.Components.CourierActivities;
using Genocs.MassTransit.Components.HttpClients;
using Genocs.MassTransit.Components.StateMachines;
using Genocs.MassTransit.Components.StateMachines.Activities;
using Genocs.MassTransit.Service;
using Genocs.MassTransit.Warehouse.Contracts;
using MassTransit;
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
        configuration.InstrumentationKey = "f28b8a8c-bf65-44a6-9976-e56613fef466";
        configuration.TelemetryInitializers.Add(new HttpDependenciesParsingTelemetryInitializer());

        _telemetryClient = new TelemetryClient(configuration);

        _module.Initialize(configuration);

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
