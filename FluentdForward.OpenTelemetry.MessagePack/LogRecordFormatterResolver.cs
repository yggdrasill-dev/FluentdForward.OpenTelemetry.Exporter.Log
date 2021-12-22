using MessagePack;
using MessagePack.Formatters;
using OpenTelemetry.Logs;

namespace FluentdForward.OpenTelemetry.MessagePack;

public class LogRecordFormatterResolver : IFormatterResolver
{
    public static readonly IFormatterResolver Instance = new LogRecordFormatterResolver();

    private Lazy<LogRecordFormatter> m_LogRecordFormatter = new Lazy<LogRecordFormatter>(() => new LogRecordFormatter());

    public IMessagePackFormatter<T>? GetFormatter<T>()
    {
        if (typeof(T) == typeof(LogRecord))
            return (IMessagePackFormatter<T>)m_LogRecordFormatter.Value;

        return null;
    }
}
