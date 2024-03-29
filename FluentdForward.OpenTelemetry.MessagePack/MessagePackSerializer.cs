﻿using FluentdForward.OpenTelemetry.Exporter.Logs;
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
		var resolvers = (formatterResolvers == null || formatterResolvers.Length == 0
			? Array.Empty<IFormatterResolver>()
			: formatterResolvers).Concat(new[] {
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
			});

		m_Options = MessagePackSerializerOptions.Standard
			.WithResolver(CompositeResolver.Create(resolvers.ToArray()));
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
