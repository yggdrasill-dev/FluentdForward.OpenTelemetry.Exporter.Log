using System.Linq;
using MessagePack;
using MessagePack.Formatters;

namespace FluentdForward.OpenTelemetry.MessagePack;

// 手寫 AOT-safe formatter，取代 [MessagePackObject] 動態（Reflection.Emit）產生的 formatter。
// 維持原本 IntKey 的 wire format（陣列），且不依賴任何 Dynamic* resolver。
// 泛型 formatter 由各消費端以封閉型別註冊（例如 new ArrayPayloadFormatter<LogRecord>()），AOT 下完全靜態。

[ExcludeFormatterFromSourceGeneratedResolver]
public sealed class ArrayPayloadFormatter<TMessage> : IMessagePackFormatter<ArrayPayload<TMessage>?>
{
	public void Serialize(ref MessagePackWriter writer, ArrayPayload<TMessage>? value, MessagePackSerializerOptions options)
	{
		if (value is null)
		{
			writer.WriteNil();
			return;
		}

		writer.WriteArrayHeader(2);
		writer.Write(value.Tag);

		var body = value.Body as IReadOnlyCollection<ArrayPayloadBody<TMessage>>
			?? value.Body?.ToArray()
			?? Array.Empty<ArrayPayloadBody<TMessage>>();

		var bodyFormatter = options.Resolver.GetFormatterWithVerify<ArrayPayloadBody<TMessage>>();

		writer.WriteArrayHeader(body.Count);
		foreach (var item in body)
			bodyFormatter.Serialize(ref writer, item, options);
	}

	public ArrayPayload<TMessage>? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) => throw new NotSupportedException();
}

[ExcludeFormatterFromSourceGeneratedResolver]
public sealed class ArrayPayloadBodyFormatter<TMessage> : IMessagePackFormatter<ArrayPayloadBody<TMessage>?>
{
	public void Serialize(ref MessagePackWriter writer, ArrayPayloadBody<TMessage>? value, MessagePackSerializerOptions options)
	{
		if (value is null)
		{
			writer.WriteNil();
			return;
		}

		writer.WriteArrayHeader(2);
		options.Resolver.GetFormatterWithVerify<ArrayPayloadMetadata>().Serialize(ref writer, value.Metadata, options);
		options.Resolver.GetFormatterWithVerify<TMessage>().Serialize(ref writer, value.Message, options);
	}

	public ArrayPayloadBody<TMessage>? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) => throw new NotSupportedException();
}

[ExcludeFormatterFromSourceGeneratedResolver]
public sealed class ArrayPayloadMetadataFormatter : IMessagePackFormatter<ArrayPayloadMetadata?>
{
	private static readonly TimestampFormatter _TimestampFormatter = new();

	public void Serialize(ref MessagePackWriter writer, ArrayPayloadMetadata? value, MessagePackSerializerOptions options)
	{
		if (value is null)
		{
			writer.WriteNil();
			return;
		}

		writer.WriteArrayHeader(2);

		// Timestamp：Unix 奈秒（與原本 [MessagePackFormatter(typeof(TimestampFormatter))] 一致）。
		_TimestampFormatter.Serialize(ref writer, value.Timestamp, options);

		// Attributes：fluentd forward 的 option map，本路徑通常為空。
		var attributes = value.Attributes;
		writer.WriteMapHeader(attributes?.Count ?? 0);
		if (attributes is not null)
			foreach (var kv in attributes)
			{
				writer.Write(kv.Key);
				MessagePackValueWriter.Write(ref writer, kv.Value);
			}
	}

	public ArrayPayloadMetadata? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) => throw new NotSupportedException();
}
