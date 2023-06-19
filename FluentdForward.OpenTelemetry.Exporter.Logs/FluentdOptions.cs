using OpenTelemetry;
using OpenTelemetry.Logs;

namespace FluentdForward.OpenTelemetry.Exporter.Logs;

/// <summary>
/// The class that defines settings for connecting to fluentd server.
/// </summary>
public class FluentdOptions
{
	/// <summary>
	/// The host of fluentd server.
	/// </summary>
	public string Host { get; set; } = "localhost";

	/// <summary>
	/// The port of fluentd server.
	/// </summary>
	public int Port { get; set; } = 24224;

	public string Tag { get; set; } = default!;

	/// <summary>
	/// The MassagePack serializer.
	/// </summary>
	public IMessagePackSerializer? Serializer { get; set; }

	public ExportProcessorType ExportProcessorType { get; set; } = ExportProcessorType.Batch;

	/// <summary>
	/// The timeout value for connecting to fluentd server. (Milliseconds)
	/// </summary>
	public int? Timeout { get; set; }

	public BatchExportProcessorOptions<LogRecord> BatchExportProcessorOptions { get; set; } = new();
}
