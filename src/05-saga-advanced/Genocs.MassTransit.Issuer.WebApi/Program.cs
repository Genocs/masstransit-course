using Genocs.MassTransit.Contracts;
using MassTransit;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();


TelemetryClient _telemetryClient;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console());

// ***********************************************
// Azure Application Insight configuration - START
builder.Services.AddApplicationInsightsTelemetry();

builder.Services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) =>
{
    module.IncludeDiagnosticSourceActivities.Add("MassTransit");

    TelemetryConfiguration configuration = TelemetryConfiguration.CreateDefault();
    configuration.ConnectionString = "InstrumentationKey=f28b8a8c-bf65-44a6-9976-e56613fef466;IngestionEndpoint=https://westeurope-5.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/";
    configuration.TelemetryInitializers.Add(new HttpDependenciesParsingTelemetryInitializer());

    _telemetryClient = new TelemetryClient(configuration);
});
// Azure Application Insight configuration - END
// ***********************************************


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<HealthCheckPublisherOptions>(options =>
{
    options.Delay = TimeSpan.FromSeconds(2);
    options.Predicate = check => check.Tags.Contains("ready");
});

builder.Services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        //cfg.Host();

        MessageDataDefaults.ExtraTimeToLive = TimeSpan.FromDays(1);
        MessageDataDefaults.Threshold = 2000;
        MessageDataDefaults.AlwaysWriteToRepository = false;

        //cfg.UseMessageData(new MongoDbMessageDataRepository(IsRunningInContainer ? "mongodb://mongo" : "mongodb://127.0.0.1", "attachments"));
    });

    //mt.AddRequestClient<SubmitOrder>(new Uri($"queue:{KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>()}"));

    x.AddRequestClient<OrderStatus>();
    x.AddRequestClient<PaymentStatus>();
    x.AddRequestClient<PaymentRequest>();

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();

Log.CloseAndFlush();