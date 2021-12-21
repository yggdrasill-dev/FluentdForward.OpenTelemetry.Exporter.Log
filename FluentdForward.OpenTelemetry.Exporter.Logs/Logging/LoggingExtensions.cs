using FluentdForward.OpenTelemetry.Exporter.Logs;
using OpenTelemetry;
using OpenTelemetry.Logs;

namespace Microsoft.Extensions.Logging;

public static class LoggingExtensions
{
    public static OpenTelemetryLoggerOptions AddFluentdForwardExporter(
        this OpenTelemetryLoggerOptions loggerOptions,
        Action<FluentdOptions>? configure = null)
    {
        var exporterOptions = new FluentdOptions();

        configure?.Invoke(exporterOptions);
        var exporter = new FluentdForwardLogExporter(exporterOptions);

        if (exporterOptions.ExportProcessorType == ExportProcessorType.Simple)
            return loggerOptions.AddProcessor(new SimpleLogRecordExportProcessor(exporter));
        else
            return loggerOptions.AddProcessor(new BatchLogRecordExportProcessor(
                exporter,
                exporterOptions.BatchExportProcessorOptions.MaxQueueSize,
                exporterOptions.BatchExportProcessorOptions.ScheduledDelayMilliseconds,
                exporterOptions.BatchExportProcessorOptions.ExporterTimeoutMilliseconds,
                exporterOptions.BatchExportProcessorOptions.MaxExportBatchSize));
    }
}
