using System.Diagnostics;
using System.Text;
using MessagePack;
using MessagePack.Formatters;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;

namespace FluentdForward.OpenTelemetry.MessagePack;

internal class LogRecordFormatter : IMessagePackFormatter<LogRecord>
{
    private static readonly IFormatterResolver _StandardResolver = MessagePackSerializerOptions.Standard.Resolver;

    public void Serialize(ref MessagePackWriter writer, LogRecord value, MessagePackSerializerOptions options)
    {
        var properties = new List<WriteProperty>();

        properties.Add((ref MessagePackWriter writer) =>
        {
            WriteHeader(ref writer, nameof(value.CategoryName), options);
            options.Resolver.GetFormatter<string>().Serialize(ref writer, value.CategoryName, options);
        });
        properties.Add((ref MessagePackWriter writer) =>
        {
            WriteHeader(ref writer, nameof(value.EventId), options);
            options.Resolver.GetFormatter<EventId>().Serialize(ref writer, value.EventId, options);
        });
        if (value.Exception != null)
            properties.Add((ref MessagePackWriter writer) =>
            {
                WriteHeader(ref writer, nameof(value.Exception), options);
                options.Resolver.GetFormatter<string>().Serialize(ref writer, value.Exception.ToString(), options);
            });
        if (value.FormattedMessage != null)
            properties.Add((ref MessagePackWriter writer) =>
            {
                WriteHeader(ref writer, nameof(value.FormattedMessage), options);
                options.Resolver.GetFormatter<string>().Serialize(ref writer, value.FormattedMessage, options);
            });
        properties.Add((ref MessagePackWriter writer) =>
        {
            WriteHeader(ref writer, nameof(value.LogLevel), options);
            options.Resolver.GetFormatter<LogLevel>().Serialize(ref writer, value.LogLevel, options);
        });
        if (value.SpanId != default)
            properties.Add((ref MessagePackWriter writer) =>
            {
                WriteHeader(ref writer, nameof(value.SpanId), options);
                options.Resolver.GetFormatter<string>().Serialize(ref writer, value.SpanId.ToString(), options);
            });
        if (value.State != null)
            properties.Add((ref MessagePackWriter writer) =>
            {
                WriteHeader(ref writer, nameof(value.State), options);

                var stateType = value.State.GetType();

                if (stateType.Name == "FormattedLogValues")
                    writer.WriteString(Encoding.UTF8.GetBytes(value.State.ToString()!));
                else
                    global::MessagePack.MessagePackSerializer.Serialize(stateType, ref writer, value.State, options);
            });
        if (value.StateValues != null)
            properties.Add((ref MessagePackWriter writer) =>
            {
                WriteHeader(ref writer, nameof(value.StateValues), options);
                options.Resolver.GetFormatter<IReadOnlyList<KeyValuePair<string, object>>>().Serialize(ref writer, value.StateValues, options);
            });
        properties.Add((ref MessagePackWriter writer) =>
        {
            WriteHeader(ref writer, nameof(value.Timestamp), options);
            options.Resolver.GetFormatter<DateTime>().Serialize(ref writer, value.Timestamp, options);
        });
        properties.Add((ref MessagePackWriter writer) =>
        {
            WriteHeader(ref writer, nameof(value.TraceFlags), options);
            options.Resolver.GetFormatter<ActivityTraceFlags>().Serialize(ref writer, value.TraceFlags, options);
        });
        if (value.TraceId != default)
            properties.Add((ref MessagePackWriter writer) =>
            {
                WriteHeader(ref writer, nameof(value.TraceId), options);
                options.Resolver.GetFormatter<string>().Serialize(ref writer, value.TraceId.ToString(), options);
            });
        if (value.TraceState != null)
            properties.Add((ref MessagePackWriter writer) =>
            {
                WriteHeader(ref writer, nameof(value.TraceState), options);
                options.Resolver.GetFormatter<string>().Serialize(ref writer, value.TraceState, options);
            });

        writer.WriteMapHeader(properties.Count);

        foreach (var func in properties)
            func(ref writer);
    }

    public LogRecord Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options) => throw new NotSupportedException();

    private void WriteHeader(ref MessagePackWriter writer, string keyName, MessagePackSerializerOptions options)
    {
        var keyNameByteArray = Encoding.UTF8.GetBytes(keyName);
        writer.WriteStringHeader(keyNameByteArray.Length);
        writer.WriteRaw(keyNameByteArray);
    }

    private delegate void WriteProperty(ref MessagePackWriter writer);
}
