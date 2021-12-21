using OpenTelemetry.Logs;

namespace FluentdForward.OpenTelemetry.Exporter.Logs;

/// <summary>
/// The interface that defines methods for serializing to send messages to fluentd server, with using the format of MessagePack.
/// </summary>
public interface IMessagePackSerializer
{
    byte[] Serialize(string tag, LogRecord message);
}
