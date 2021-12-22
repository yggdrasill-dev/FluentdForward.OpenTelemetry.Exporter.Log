using System.Text;
using MessagePack;
using MessagePack.Formatters;

namespace FluentdForward.OpenTelemetry.MessagePack;

internal class TimeFormatter
    : IMessagePackFormatter<DateTime>,
    IMessagePackFormatter<DateTimeOffset>,
    IMessagePackFormatter<DateTime?>,
    IMessagePackFormatter<DateTimeOffset?>
{
    public void Serialize(ref MessagePackWriter writer, DateTime value, MessagePackSerializerOptions options)
    {
        writer.WriteString(Encoding.UTF8.GetBytes(value.ToUniversalTime().ToString("O")));
    }

    public DateTime Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) => throw new NotSupportedException();

    public void Serialize(ref MessagePackWriter writer, DateTimeOffset value, MessagePackSerializerOptions options)
    {
        writer.WriteString(Encoding.UTF8.GetBytes(value.ToUniversalTime().ToString("O")));
    }

    DateTimeOffset IMessagePackFormatter<DateTimeOffset>.Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        => throw new NotSupportedException();

    public void Serialize(ref MessagePackWriter writer, DateTimeOffset? value, MessagePackSerializerOptions options)
    {
        if (value == null)
            writer.WriteNil();
        else
            writer.WriteString(Encoding.UTF8.GetBytes(value.Value.ToUniversalTime().ToString("O")));
    }

    DateTimeOffset? IMessagePackFormatter<DateTimeOffset?>.Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) => throw new NotSupportedException();

    public void Serialize(ref MessagePackWriter writer, DateTime? value, MessagePackSerializerOptions options)
    {
        if (value == null)
            writer.WriteNil();
        else
            writer.WriteString(Encoding.UTF8.GetBytes(value.Value.ToUniversalTime().ToString("O")));
    }

    DateTime? IMessagePackFormatter<DateTime?>.Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        => throw new NotSupportedException();
}
