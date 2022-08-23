using MessagePack;
using MessagePack.Formatters;
using OpenTelemetry.Logs;

namespace FluentdForward.OpenTelemetry.MessagePack;

public class LogRecordFormatterResolver : IFormatterResolver
{
	public static readonly IFormatterResolver Instance = new LogRecordFormatterResolver(() => new LogRecordFormatter());

	private readonly Lazy<LogRecordFormatter> m_LogRecordFormatter;
	private readonly Lazy<TimeFormatter> m_TimeFormatter = new(() => new TimeFormatter());

	private LogRecordFormatterResolver(Func<LogRecordFormatter> logRecordFormatterResolver)
	{
		m_LogRecordFormatter = new Lazy<LogRecordFormatter>(logRecordFormatterResolver);
	}

	public static IFormatterResolver GetResolverInstanceWithExtendInfo(string extendPropertyName, Func<object> extendObjectResolver)
		=> new LogRecordFormatterResolver(
			   () => new LogRecordFormatter(extendPropertyName, extendObjectResolver));

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
