﻿using System.Diagnostics;
using System.Text;
using MessagePack;
using MessagePack.Formatters;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;

namespace FluentdForward.OpenTelemetry.MessagePack;

internal class LogRecordFormatter : IMessagePackFormatter<LogRecord>
{
	private readonly string? m_ExtendPropertyName;
	private readonly Func<object?> m_ExtendObjectResolver = () => null;

	public LogRecordFormatter()
	{
	}

	public LogRecordFormatter(string extendPropertyName, Func<object> extendObjectResolver)
	{
		m_ExtendPropertyName = extendPropertyName;
		m_ExtendObjectResolver = extendObjectResolver;
	}

	public void Serialize(ref MessagePackWriter writer, LogRecord value, MessagePackSerializerOptions options)
	{
		var properties = MakeValueToWriteProperties(value, options);

		if (!string.IsNullOrWhiteSpace(m_ExtendPropertyName))
		{
			var propertyName = m_ExtendPropertyName!;
			var extendValue = m_ExtendObjectResolver();

			if (extendValue is not null)
				properties.Add((ref MessagePackWriter writer) =>
				{
					LogRecordFormatter.WriteHeader(ref writer, propertyName);

					global::MessagePack.MessagePackSerializer.Serialize(extendValue.GetType(), ref writer, extendValue, options);
				});
		}

		writer.WriteMapHeader(properties.Count);

		foreach (var propertyWriteFunc in properties)
			propertyWriteFunc(ref writer);
	}

	public LogRecord Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) => throw new NotSupportedException();

	private static List<WriteProperty> MakeValueToWriteProperties(LogRecord value, MessagePackSerializerOptions options)
	{
		var properties = new List<WriteProperty>
		{
			(ref MessagePackWriter writer) =>
			{
				LogRecordFormatter.WriteHeader(ref writer, nameof(value.CategoryName));
				options.Resolver.GetFormatter<string?>()!.Serialize(ref writer, value.CategoryName, options);
			},
			(ref MessagePackWriter writer) =>
			{
				LogRecordFormatter.WriteHeader(ref writer, nameof(value.EventId));
				options.Resolver.GetFormatter<EventId>()!.Serialize(ref writer, value.EventId, options);
			}
		};
		if (value.Exception != null)
		{
			properties.Add((ref MessagePackWriter writer) =>
			{
				LogRecordFormatter.WriteHeader(ref writer, nameof(value.Exception));
				options.Resolver.GetFormatter<string>()!.Serialize(ref writer, value.Exception.ToString(), options);
			});
			properties.Add((ref MessagePackWriter writer) =>
			{
				LogRecordFormatter.WriteHeader(ref writer, $"{nameof(value.Exception)}Type");
				var type = value.Exception.GetType();
				options.Resolver.GetFormatter<string>()!.Serialize(ref writer, type.FullName ?? type.Name, options);
			});
		}

		if (value.FormattedMessage != null)
			properties.Add((ref MessagePackWriter writer) =>
			{
				LogRecordFormatter.WriteHeader(ref writer, nameof(value.FormattedMessage));
				options.Resolver.GetFormatter<string>()!.Serialize(ref writer, value.FormattedMessage, options);
			});
		properties.Add((ref MessagePackWriter writer) =>
		{
			LogRecordFormatter.WriteHeader(ref writer, nameof(value.LogLevel));
			options.Resolver.GetFormatter<LogLevel>()!.Serialize(ref writer, value.LogLevel, options);
		});
		if (value.SpanId != default)
			properties.Add((ref MessagePackWriter writer) =>
			{
				LogRecordFormatter.WriteHeader(ref writer, nameof(value.SpanId));
				options.Resolver.GetFormatter<string>()!.Serialize(ref writer, value.SpanId.ToString(), options);
			});
		if (value.Attributes != null)
			properties.Add((ref MessagePackWriter writer) =>
			{
				LogRecordFormatter.WriteHeader(ref writer, nameof(value.Attributes));
				var dict = new Dictionary<string, object>();

				writer.WriteArrayHeader(value.Attributes.Count);
				foreach (var kv in value.Attributes)
				{
					if (kv.Value is null)
						continue;

					var keyName = kv.Key;
					if (string.IsNullOrEmpty(keyName))
						keyName = "{source}";

					writer.WriteMapHeader(1);
					LogRecordFormatter.WriteHeader(ref writer, keyName);
					global::MessagePack.MessagePackSerializer.Serialize(kv.Value.GetType(), ref writer, kv.Value, options);
				}
			});
		properties.Add((ref MessagePackWriter writer) =>
		{
			LogRecordFormatter.WriteHeader(ref writer, nameof(value.Timestamp));
			options.Resolver.GetFormatter<DateTime>()!.Serialize(ref writer, value.Timestamp, options);
		});
		properties.Add((ref MessagePackWriter writer) =>
		{
			LogRecordFormatter.WriteHeader(ref writer, nameof(value.TraceFlags));
			options.Resolver.GetFormatter<ActivityTraceFlags>()!.Serialize(ref writer, value.TraceFlags, options);
		});
		if (value.TraceId != default)
			properties.Add((ref MessagePackWriter writer) =>
			{
				LogRecordFormatter.WriteHeader(ref writer, nameof(value.TraceId));
				options.Resolver.GetFormatter<string>()!.Serialize(ref writer, value.TraceId.ToString(), options);
			});
		if (value.TraceState != null)
			properties.Add((ref MessagePackWriter writer) =>
			{
				LogRecordFormatter.WriteHeader(ref writer, nameof(value.TraceState));
				options.Resolver.GetFormatter<string>()!.Serialize(ref writer, value.TraceState, options);
			});
		return properties;
	}

	private static void WriteHeader(ref MessagePackWriter writer, string keyName)
	{
		var keyNameByteArray = Encoding.UTF8.GetBytes(keyName);
		writer.WriteStringHeader(keyNameByteArray.Length);
		writer.WriteRaw(keyNameByteArray);
	}

	private delegate void WriteProperty(ref MessagePackWriter writer);
}
