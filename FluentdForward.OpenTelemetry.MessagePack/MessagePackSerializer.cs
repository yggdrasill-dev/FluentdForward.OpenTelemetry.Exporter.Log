using FluentdForward.OpenTelemetry.Exporter.Logs;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using OpenTelemetry;
using OpenTelemetry.Logs;

namespace FluentdForward.OpenTelemetry.MessagePack;

internal class MessagePackSerializer : IMessagePackSerializer
{
	private readonly MessagePackSerializerOptions m_Options;

	/// <summary>
	/// Create a new <see cref="MessagePackSerializer"/> instance.
	/// </summary>
	public MessagePackSerializer(IFormatterResolver[]? formatterResolvers = null)
	{
		// AOT-safe resolver 鏈：完全不使用會走 Reflection.Emit 的 Dynamic* resolver。
		// 手寫 formatter（payload 走 IntKey 陣列、EventId 走 {Id,Name} map）優先，其後為：
		// - 呼叫端傳入的 resolver（含 extend 資訊的 LogRecordFormatterResolver）
		// - LogRecordFormatterResolver：LogRecord 與 DateTime（TimeFormatter）
		// - Builtin / Attribute：string、數值等內建靜態 formatter
		var formatters = new IMessagePackFormatter[]
		{
			new ArrayPayloadFormatter<LogRecord>(),
			new ArrayPayloadBodyFormatter<LogRecord>(),
			new ArrayPayloadMetadataFormatter(),
			new EventIdFormatter(),
		};

		var resolvers = (formatterResolvers == null || formatterResolvers.Length == 0
			? Array.Empty<IFormatterResolver>()
			: formatterResolvers).Concat(new[] {
				LogRecordFormatterResolver.Instance,
				BuiltinResolver.Instance,
				AttributeFormatterResolver.Instance,
			});

		m_Options = MessagePackSerializerOptions.Standard
			.WithResolver(CompositeResolver.Create(formatters, resolvers.ToArray()));
	}

	/// <inheritdoc cref="IMessagePackSerializer.Serialize{T}(string, T)" />
	public byte[] Serialize(string tag, Batch<LogRecord> batch)
	{
		var payload = new ArrayPayload<LogRecord>
		{
			Tag = tag,
			Body = ReadAsPayloadBody(batch)
		};

		return global::MessagePack.MessagePackSerializer.Serialize(payload, m_Options);
	}

	private static IEnumerable<ArrayPayloadBody<LogRecord>> ReadAsPayloadBody(Batch<LogRecord> batch)
	{
		foreach (var record in batch)
			yield return new ArrayPayloadBody<LogRecord>
			{
				Metadata = new ArrayPayloadMetadata
				{
					Timestamp = record.Timestamp
				},
				Message = record
			};
	}
}
