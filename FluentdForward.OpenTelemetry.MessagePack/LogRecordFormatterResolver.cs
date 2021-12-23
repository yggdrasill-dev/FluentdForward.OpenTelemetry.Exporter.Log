using MessagePack;
using MessagePack.Formatters;
using OpenTelemetry.Logs;

namespace FluentdForward.OpenTelemetry.MessagePack;

public class LogRecordFormatterResolver : IFormatterResolver
{
	public static readonly IFormatterResolver Instance = new LogRecordFormatterResolver();

	private Lazy<LogRecordFormatter> m_LogRecordFormatter = new(() => new LogRecordFormatter());
	private Lazy<TimeFormatter> m_TimeFormatter = new(() => new TimeFormatter());

	public IMessagePackFormatter<T>? GetFormatter<T>()
	{
		if (typeof(T) == typeof(LogRecord))
			return (IMessagePackFormatter<T>)m_LogRecordFormatter.Value;

		if (typeof(T) == typeof(DateTime)
			|| typeof(T) == typeof(DateTimeOffset)
			|| typeof(T) == typeof(DateTime?)
			|| typeof(T) == typeof(DateTimeOffset?))
			return (IMessagePackFormatter<T>)m_TimeFormatter.Value;

		return null;
	}
}
