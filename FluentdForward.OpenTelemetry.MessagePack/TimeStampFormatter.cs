using MessagePack;
using MessagePack.Formatters;

namespace FluentdForward.OpenTelemetry.MessagePack;

public class TimeStampFormatter :
	IMessagePackFormatter<DateTime>,
	IMessagePackFormatter<DateTimeOffset>
{
	public void Serialize(ref MessagePackWriter writer, DateTime value, MessagePackSerializerOptions options)
		=> writer.WriteInt64(new DateTimeOffset(
			value.ToUniversalTime(),
			TimeSpan.Zero).ToUnixTimeSeconds());

	public DateTime Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
		=> throw new NotSupportedException();

	public void Serialize(ref MessagePackWriter writer, DateTimeOffset value, MessagePackSerializerOptions options)
		=> writer.WriteInt64(value.ToUnixTimeSeconds());

	DateTimeOffset IMessagePackFormatter<DateTimeOffset>.Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
		=> throw new NotSupportedException();
}
