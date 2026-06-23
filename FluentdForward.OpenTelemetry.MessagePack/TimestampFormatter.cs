using MessagePack;
using MessagePack.Formatters;

namespace FluentdForward.OpenTelemetry.MessagePack;

// 由 [MessagePackFormatter] 屬性明確指定使用，不讓 source generator 自動納入產生的 resolver
// （與 TimeFormatter 同樣是 DateTime/DateTimeOffset formatter，否則會觸發 MsgPack009 多重 formatter 衝突）。
[ExcludeFormatterFromSourceGeneratedResolver]
public class TimestampFormatter :
	IMessagePackFormatter<DateTime>,
	IMessagePackFormatter<DateTimeOffset>
{
	public void Serialize(ref MessagePackWriter writer, DateTime value, MessagePackSerializerOptions options)
		=> writer.WriteInt64(new DateTimeOffset(
			value.ToUniversalTime(),
			TimeSpan.Zero).ToUnixTimeMilliseconds() * 1000000L);

	public DateTime Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
		=> throw new NotSupportedException();

	public void Serialize(ref MessagePackWriter writer, DateTimeOffset value, MessagePackSerializerOptions options)
		=> writer.WriteInt64(value.ToUnixTimeMilliseconds() * 1000000L);

	DateTimeOffset IMessagePackFormatter<DateTimeOffset>.Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
		=> throw new NotSupportedException();
}
