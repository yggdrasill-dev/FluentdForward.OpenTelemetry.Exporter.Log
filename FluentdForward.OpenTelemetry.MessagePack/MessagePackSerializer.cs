using FluentdForward.OpenTelemetry.Exporter.Logs;
using MessagePack;
using MessagePack.Resolvers;
using OpenTelemetry.Logs;

namespace FluentdForward.OpenTelemetry.MessagePack;

internal class MessagePackSerializer : IMessagePackSerializer
{
	private readonly MessagePackSerializerOptions m_Options;

	/// <summary>
	/// Create a new <see cref="MessagePackSerializer"/> instance.
	/// </summary>
	public MessagePackSerializer(IFormatterResolver[] formatterResolvers)
	{
		if (formatterResolvers == null || formatterResolvers.Length == 0)
			m_Options = MessagePackSerializerOptions.Standard
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
			}));
		else
			m_Options = MessagePackSerializerOptions.Standard
				.WithResolver(CompositeResolver.Create(formatterResolvers));
	}

	/// <inheritdoc cref="IMessagePackSerializer.Serialize{T}(string, T)" />
	public byte[] Serialize(string tag, LogRecord message)
	{
		var payload = new Payload<LogRecord>
		{
			Tag = tag,
			Timestamp = message.Timestamp,
			Message = message
		};

		return global::MessagePack.MessagePackSerializer.Serialize(payload, m_Options);
	}
}
