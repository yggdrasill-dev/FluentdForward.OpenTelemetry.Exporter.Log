using MessagePack;
using MessagePack.Formatters;
using OpenTelemetry.Logs;

namespace FluentdForward.OpenTelemetry.MessagePack;

public class LogRecordFormatterResolver : IFormatterResolver
{
	public static readonly IFormatterResolver Instance = new LogRecordFormatterResolver();

	private readonly Lazy<LogRecordFormatter> m_LogRecordFormatter = new(() => new LogRecordFormatter());
	private readonly Lazy<TimeFormatter> m_TimeFormatter = new(() => new TimeFormatter());

	private LogRecordFormatterResolver()
	{
	}

	public IMessagePackFormatter<T>? GetFormatter<T>()
	{
		return typeof(T) == typeof(LogRecord)
			? (IMessagePackFormatter<T>)m_LogRecordFormatter.Value
			: typeof(T) == typeof(DateTime)
			|| typeof(T) == typeof(DateTimeOffset)
			|| typeof(T) == typeof(DateTime?)
			|| typeof(T) == typeof(DateTimeOffset?)
			? (IMessagePackFormatter<T>)m_TimeFormatter.Value
			: null;
	}
}
