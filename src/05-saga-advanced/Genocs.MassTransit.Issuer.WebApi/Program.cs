using Genocs.MassTransit.Contracts;
using MassTransit;
using Microsoft.ApplicationInsights;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Events;
using Azure.Monitor.OpenTelemetry.Exporter;
using MassTransit.Metadata;
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


TelemetryClient _telemetryClient;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console());

// ***********************************************
// Open Telemetry - START
builder.Services.AddOpenTelemetryTracing(x =>
{
    x.SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService("IssuerApi")
            .AddTelemetrySdk()
            .AddEnvironmentVariableDetector())
        .AddSource("MassTransit")
        .AddAspNetCoreInstrumentation()
        .AddAzureMonitorTraceExporter(o =>
        {
            o.ConnectionString = builder.Configuration["ApplicationInsightsConnectionString"];
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
// Open Telemetry - END
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

    //x.AddMongoDbOutbox(o =>
    //{
    //    o.DisableInboxCleanupService();
    //    o.ClientFactory(provider => provider.GetRequiredService<IMongoClient>());
    //    o.DatabaseFactory(provider => provider.GetRequiredService<IMongoDatabase>());

    //    o.UseBusOutbox(bo => bo.DisableDeliveryService());
    //});

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