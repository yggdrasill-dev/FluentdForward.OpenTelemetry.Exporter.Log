using FluentdForward.OpenTelemetry.Exporter.Logs;
using OpenTelemetry.Logs;

namespace FluentdForward.OpenTelemetry.MessagePack;

internal class MessagePackSerializer : IMessagePackSerializer
{
    /// <summary>
    /// Create a new <see cref="MessagePackSerializer"/> instance.
    /// </summary>
    public MessagePackSerializer()
    { }

    /// <inheritdoc cref="IMessagePackSerializer.Serialize{T}(string, T)" />
    public byte[] Serialize(string tag, LogRecord message)
    {
        var payload = new Payload<LogRecord>
        {
            Tag = tag,
            Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
            Message = message
        };

        return global::MessagePack.MessagePackSerializer.Serialize(payload);
    }
}
