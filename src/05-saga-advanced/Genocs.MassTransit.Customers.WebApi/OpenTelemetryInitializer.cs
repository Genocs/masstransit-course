using Azure.Monitor.OpenTelemetry.Exporter;
using MassTransit.Metadata;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace Genocs.MassTransit.Customers.WebApi;

internal static class OpenTelemetryInitializer
{
    public static void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetryTracing(x =>
        {
            x.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService("CustomerApi")
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
    }
}
