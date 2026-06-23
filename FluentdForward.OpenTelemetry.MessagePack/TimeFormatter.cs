using System.Text;
using MessagePack;
using MessagePack.Formatters;

namespace FluentdForward.OpenTelemetry.MessagePack;

// 由 LogRecordFormatterResolver 明確提供，不讓 source generator 自動納入產生的 resolver
// （與 TimestampFormatter 同樣是 DateTime/DateTimeOffset formatter，否則會觸發 MsgPack009 多重 formatter 衝突）。
[ExcludeFormatterFromSourceGeneratedResolver]
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
