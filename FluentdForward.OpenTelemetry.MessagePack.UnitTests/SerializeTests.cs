using System;
using System.Collections.Generic;
using System.Reflection;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTelemetry.Logs;

namespace FluentdForward.OpenTelemetry.MessagePack.UnitTests;

[TestClass]
public class SerializeTests
{
    private MessagePackSerializerOptions m_SerializerOptions = MessagePackSerializerOptions.Standard.WithResolver(CompositeResolver.Create(
        new[] {
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

    [TestMethod]
    public void LogRecord_Serialize_Test()
    {
        var record = (LogRecord)Activator.CreateInstance(
            typeof(LogRecord),
            BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new object?[] {
                default(IExternalScopeProvider),
                DateTime.Parse("2021-12-22T05:43:42.5945187Z"),
                "Test.Serialize",
                LogLevel.Information,
                new EventId(1),
                "aabbcc",
                new{
                    m = "aabbcc"
                },
                default(Exception),
                default(IReadOnlyList<KeyValuePair<string, object>>)
            },
            null,
            null)!;

        var expected = "{\"CategoryName\":\"Test.Serialize\",\"EventId\":{\"Id\":1,\"Name\":null},\"FormattedMessage\":\"aabbcc\",\"LogLevel\":\"Information\",\"State\":{\"m\":\"aabbcc\"},\"Timestamp\":\"2021-12-22T05:43:42.5945187Z\",\"TraceFlags\":\"None\"}";

        var bytes = global::MessagePack.MessagePackSerializer.Serialize(record, m_SerializerOptions);
        var actual = global::MessagePack.MessagePackSerializer.ConvertToJson(bytes);

        Assert.AreEqual(expected, actual);
    }

    //[TestMethod]
    //public void LogRecord_With_FormattedLogValues_Serialize_Test()
    //{
    //    var record = (LogRecord)Activator.CreateInstance(
    //        typeof(LogRecord),
    //        BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Instance,
    //        null,
    //        new object?[] {
    //            default(IExternalScopeProvider),
    //            DateTime.Parse("2021-12-22T05:43:42.5945187Z"),
    //            "Test.Serialize",
    //            LogLevel.Information,
    //            new EventId(1),
    //            "aabbcc",
    //            new FormattedLogValues("aabbcc"),
    //            default(Exception),
    //            default(IReadOnlyList<KeyValuePair<string, object>>)
    //        },
    //        null,
    //        null)!;

    //    var expected = "{\"CategoryName\":\"Test.Serialize\",\"EventId\":{\"Id\":1,\"Name\":null},\"FormattedMessage\":\"aabbcc\",\"LogLevel\":\"Information\",\"State\":{\"m\":\"aabbcc\"},\"Timestamp\":\"2021-12-22T05:43:42.5945187Z\",\"TraceFlags\":\"None\"}";

    //    var bytes = global::MessagePack.MessagePackSerializer.Serialize(record, m_SerializerOptions);
    //    var actual = global::MessagePack.MessagePackSerializer.ConvertToJson(bytes);

    //    Assert.AreEqual(expected, actual);
    //}

    [TestMethod]
    public void Payload_Serialize_Test()
    {
        var payload = new Payload<object?>
        {
            Tag = "test",
            Timestamp = DateTime.SpecifyKind(
                DateTimeOffset.FromUnixTimeMilliseconds(1640192104286).UtcDateTime,
                DateTimeKind.Utc),
            Message = null
        };

        var expected = "[\"test\",1640192104,null]";

        var bytes = global::MessagePack.MessagePackSerializer.Serialize(payload, m_SerializerOptions);
        var actual = global::MessagePack.MessagePackSerializer.ConvertToJson(bytes);

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Payload_With_LogRecord_Serialize_Test()
    {
        var record = (LogRecord)Activator.CreateInstance(
            typeof(LogRecord),
            BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new object?[] {
                default(IExternalScopeProvider),
                DateTime.Parse("2021-12-22T05:43:42.5945187Z"),
                "Test.Serialize",
                LogLevel.Information,
                new EventId(1),
                "aabbcc",
                new{
                    m = "aabbcc"
                },
                default(Exception),
                default(IReadOnlyList<KeyValuePair<string, object>>)
            },
            null,
            null)!;

        var payload = new Payload<LogRecord>
        {
            Tag = "test",
            Timestamp = record.Timestamp,
            Message = record
        };

        var expected = "[\"test\",1640180622,{\"CategoryName\":\"Test.Serialize\",\"EventId\":{\"Id\":1,\"Name\":null},\"FormattedMessage\":\"aabbcc\",\"LogLevel\":\"Information\",\"State\":{\"m\":\"aabbcc\"},\"Timestamp\":\"2021-12-22T05:43:42.5945187Z\",\"TraceFlags\":\"None\"}]";

        var bytes = global::MessagePack.MessagePackSerializer.Serialize(payload, m_SerializerOptions);
        var actual = global::MessagePack.MessagePackSerializer.ConvertToJson(bytes);

        Assert.AreEqual(expected, actual);
    }

    //[TestMethod]
    //public void FormattedLogValue_Serialize_Test()
    //{
    //    var value = new FormattedLogValues("Test");

    //    var expected = "\"Test\"";

    //    var bytes = global::MessagePack.MessagePackSerializer.Serialize(value, m_SerializerOptions);
    //    var actual = global::MessagePack.MessagePackSerializer.ConvertToJson(bytes);

    //    Assert.AreEqual(expected, actual);
    //}
}
