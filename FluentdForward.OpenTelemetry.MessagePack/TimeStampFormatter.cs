using MessagePack;
using MessagePack.Formatters;

namespace FluentdForward.OpenTelemetry.MessagePack;

public class TimeStampFormatter :
	global::MessagePack.Formatters.IMessagePackFormatter<DateTime>,
	global::MessagePack.Formatters.IMessagePackFormatter<DateTimeOffset>
{
	public void Serialize(ref MessagePackWriter writer, DateTime value, MessagePackSerializerOptions options)
	{
		var timestamp = value - DateTime.UnixEpoch;

		writer.WriteInt64((long)timestamp.TotalSeconds);
	}

	public DateTime Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) => throw new NotSupportedException();

	public void Serialize(ref MessagePackWriter writer, DateTimeOffset value, MessagePackSerializerOptions options)
	{
		writer.WriteInt64(value.ToUnixTimeSeconds());
	}

	DateTimeOffset IMessagePackFormatter<DateTimeOffset>.Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
		=> throw new NotSupportedException();
}
