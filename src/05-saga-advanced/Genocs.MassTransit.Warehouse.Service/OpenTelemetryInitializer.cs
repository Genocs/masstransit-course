using Azure.Monitor.OpenTelemetry.Exporter;
using MassTransit.Metadata;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace Genocs.MassTransit.Warehouse.Service;

internal static class OpenTelemetryInitializer
{
    public static void Initialize(HostBuilderContext hostContext, IServiceCollection services)
    {
        services.AddOpenTelemetryTracing(builder =>
        {
            builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService("WarehouseService")
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
    }
}
