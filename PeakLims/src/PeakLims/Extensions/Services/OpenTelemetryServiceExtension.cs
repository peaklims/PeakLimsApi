namespace PeakLims.Extensions.Services;

using PeakLims.Services;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Resources;

public static class OpenTelemetryServiceExtension
{
    public static void OpenTelemetryRegistration(this WebApplicationBuilder builder, IConfiguration configuration, string serviceName)
    {
        var resourceBuilder = ResourceBuilder.CreateDefault().AddService(serviceName)
            .AddTelemetrySdk()
            .AddEnvironmentVariableDetector();
        
        builder.Logging.AddOpenTelemetry(o =>
        {
            // TODO: Setup an exporter here
            o.SetResourceBuilder(resourceBuilder);
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metricsBuilder =>
                metricsBuilder.SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEventCountersInstrumentation(c =>
                    {
                        // https://learn.microsoft.com/en-us/dotnet/core/diagnostics/available-counters
                        c.AddEventSources(
                            "Microsoft.AspNetCore.Hosting",
                            "Microsoft-AspNetCore-Server-Kestrel",
                            "System.Net.Http",
                            "System.Net.Sockets",
                            "System.Net.NameResolution",
                            "System.Net.Security");
                    }))
            .WithTracing(tracerBuilder =>
                tracerBuilder.SetResourceBuilder(resourceBuilder)
                    .AddSource("MassTransit")
                    .AddSource("Npgsql")
                    .AddSqlClientInstrumentation(opt => opt.SetDbStatementForText = true)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddJaegerExporter(o =>
                    {
                        o.AgentHost = configuration.GetJaegerHost();
                        o.AgentPort = 58157;
                        o.MaxPayloadSizeInBytes = 4096;
                        o.ExportProcessorType = ExportProcessorType.Batch;
                        o.BatchExportProcessorOptions = new BatchExportProcessorOptions<System.Diagnostics.Activity>
                        {
                            MaxQueueSize = 2048,
                            ScheduledDelayMilliseconds = 5000,
                            ExporterTimeoutMilliseconds = 30000,
                            MaxExportBatchSize = 512,
                        };
                    }));
    }
}