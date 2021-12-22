using MessagePack;

namespace FluentdForward.OpenTelemetry.MessagePack;

public class TimeStampFormatter : global::MessagePack.Formatters.IMessagePackFormatter<DateTime>
{
    public void Serialize(ref MessagePackWriter writer, DateTime value, MessagePackSerializerOptions options)
    {
        var timestamp = value - DateTime.UnixEpoch;

        writer.WriteInt64((long)timestamp.TotalSeconds);
    }

    public DateTime Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) => throw new NotSupportedException();
}
