using FluentdForward.OpenTelemetry.Exporter.Logs;
using MessagePack;
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
		m_Options = formatterResolvers == null || formatterResolvers.Length == 0
			? MessagePackSerializerOptions.Standard
				.WithResolver(CompositeResolver.Create(new[] {
				LogRecordFormatterResolver.Instance,
				BuiltinResolver.Instance,
				AttributeFormatterResolver.Instance,

				// replace enum resolver
				DynamicEnumAsStringResolver.Instance,

				DynamicGenericResolver.Instance,
				DynamicUnionResolver.Instance,
				DynamicObjectResolver.Instance,

				PrimitiveObjectResolver.Instance,

				// final fallback(last priority)
				DynamicContractlessObjectResolver.Instance,
			}))
			: MessagePackSerializerOptions.Standard
				.WithResolver(CompositeResolver.Create(formatterResolvers));
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
				Timestamp = record.Timestamp,
				Message = record
			};
	}
}
