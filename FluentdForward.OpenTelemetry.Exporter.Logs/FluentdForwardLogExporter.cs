using OpenTelemetry;
using OpenTelemetry.Logs;

namespace FluentdForward.OpenTelemetry.Exporter.Logs;

internal class FluentdForwardLogExporter : BaseExporter<LogRecord>
{
	private readonly FluentdClient m_ExportClient;
	private readonly FluentdOptions m_FluentdOptions;

	public FluentdForwardLogExporter(FluentdOptions fluentdOptions)
	{
		m_FluentdOptions = fluentdOptions ?? throw new ArgumentNullException(nameof(fluentdOptions));
		m_ExportClient = new FluentdClient(fluentdOptions);
	}

	public override ExportResult Export(in Batch<LogRecord> batch)
	{
		// Prevents the exporter's gRPC and HTTP operations from being instrumented.
		using var scope = SuppressInstrumentationScope.Begin();

		try
		{
			m_ExportClient.SendAsync(m_FluentdOptions.Tag, batch, default)
				.GetAwaiter()
				.GetResult();

			return ExportResult.Success;
		}
		catch (Exception ex)
		{
			FluentdForwardExporterEventSource.Log.ExportMethodException(ex);
			return ExportResult.Failure;
		}
	}
}
