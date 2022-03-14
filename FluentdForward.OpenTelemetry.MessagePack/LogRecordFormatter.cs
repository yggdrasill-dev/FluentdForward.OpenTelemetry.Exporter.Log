using System.Diagnostics;
using System.Text;
using MessagePack;
using MessagePack.Formatters;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;

namespace FluentdForward.OpenTelemetry.MessagePack;

internal class LogRecordFormatter : IMessagePackFormatter<LogRecord>
{
	public void Serialize(ref MessagePackWriter writer, LogRecord value, MessagePackSerializerOptions options)
	{
		var properties = new List<WriteProperty>
		{
			(ref MessagePackWriter writer) =>
			{
				LogRecordFormatter.WriteHeader(ref writer, nameof(value.CategoryName));
				options.Resolver.GetFormatter<string>().Serialize(ref writer, value.CategoryName, options);
			},
			(ref MessagePackWriter writer) =>
			{
				LogRecordFormatter.WriteHeader(ref writer, nameof(value.EventId));
				options.Resolver.GetFormatter<EventId>().Serialize(ref writer, value.EventId, options);
			}
		};
		if (value.Exception != null)
			properties.Add((ref MessagePackWriter writer) =>
			{
				LogRecordFormatter.WriteHeader(ref writer, nameof(value.Exception));
				options.Resolver.GetFormatter<string>().Serialize(ref writer, value.Exception.ToString(), options);
			});
		if (value.FormattedMessage != null)
			properties.Add((ref MessagePackWriter writer) =>
			{
				LogRecordFormatter.WriteHeader(ref writer, nameof(value.FormattedMessage));
				options.Resolver.GetFormatter<string>().Serialize(ref writer, value.FormattedMessage, options);
			});
		properties.Add((ref MessagePackWriter writer) =>
		{
			LogRecordFormatter.WriteHeader(ref writer, nameof(value.LogLevel));
			options.Resolver.GetFormatter<LogLevel>().Serialize(ref writer, value.LogLevel, options);
		});
		if (value.SpanId != default)
			properties.Add((ref MessagePackWriter writer) =>
			{
				LogRecordFormatter.WriteHeader(ref writer, nameof(value.SpanId));
				options.Resolver.GetFormatter<string>().Serialize(ref writer, value.SpanId.ToString(), options);
			});
		if (value.State != null)
			properties.Add((ref MessagePackWriter writer) =>
			{
				LogRecordFormatter.WriteHeader(ref writer, nameof(value.State));

				var stateType = value.State.GetType();

				if (stateType.Name == "FormattedLogValues")
					global::MessagePack.MessagePackSerializer.Serialize(ref writer, new { Message = value.State.ToString()! }, options);
				else
					global::MessagePack.MessagePackSerializer.Serialize(stateType, ref writer, value.State, options);
			});
		if (value.StateValues != null)
			properties.Add((ref MessagePackWriter writer) =>
			{
				LogRecordFormatter.WriteHeader(ref writer, nameof(value.StateValues));
				var dict = new Dictionary<string, object>();

				writer.WriteArrayHeader(value.StateValues.Count);
				foreach (var kv in value.StateValues)
				{
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
			options.Resolver.GetFormatter<DateTime>().Serialize(ref writer, value.Timestamp, options);
		});
		properties.Add((ref MessagePackWriter writer) =>
		{
			LogRecordFormatter.WriteHeader(ref writer, nameof(value.TraceFlags));
			options.Resolver.GetFormatter<ActivityTraceFlags>().Serialize(ref writer, value.TraceFlags, options);
		});
		if (value.TraceId != default)
			properties.Add((ref MessagePackWriter writer) =>
			{
				LogRecordFormatter.WriteHeader(ref writer, nameof(value.TraceId));
				options.Resolver.GetFormatter<string>().Serialize(ref writer, value.TraceId.ToString(), options);
			});
		if (value.TraceState != null)
			properties.Add((ref MessagePackWriter writer) =>
			{
				LogRecordFormatter.WriteHeader(ref writer, nameof(value.TraceState));
				options.Resolver.GetFormatter<string>().Serialize(ref writer, value.TraceState, options);
			});

		writer.WriteMapHeader(properties.Count);

		foreach (var func in properties)
			func(ref writer);
	}

	public LogRecord Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) => throw new NotSupportedException();

	private static void WriteHeader(ref MessagePackWriter writer, string keyName)
	{
		var keyNameByteArray = Encoding.UTF8.GetBytes(keyName);
		writer.WriteStringHeader(keyNameByteArray.Length);
		writer.WriteRaw(keyNameByteArray);
	}

	private delegate void WriteProperty(ref MessagePackWriter writer);
}
