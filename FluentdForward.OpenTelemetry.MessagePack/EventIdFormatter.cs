using MessagePack;
using MessagePack.Formatters;
using Microsoft.Extensions.Logging;

namespace FluentdForward.OpenTelemetry.MessagePack;

// Microsoft.Extensions.Logging.EventId 沒有 [MessagePackObject]，原本靠動態 resolver（Reflection.Emit）
// 序列化，AOT 下會失敗。提供明確 formatter，輸出 {"Id":..,"Name":..}，與原動態序列化結果一致。
[ExcludeFormatterFromSourceGeneratedResolver]
internal sealed class EventIdFormatter : IMessagePackFormatter<EventId>
{
	public void Serialize(ref MessagePackWriter writer, EventId value, MessagePackSerializerOptions options)
	{
		writer.WriteMapHeader(2);
		writer.Write("Id");
		writer.Write(value.Id);
		writer.Write("Name");
		if (value.Name is null)
			writer.WriteNil();
		else
			writer.Write(value.Name);
	}

	public EventId Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) => throw new NotSupportedException();
}
