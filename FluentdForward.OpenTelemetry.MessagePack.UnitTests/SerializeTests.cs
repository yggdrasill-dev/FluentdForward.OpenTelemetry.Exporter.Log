using System.Collections.Generic;
using System.Reflection;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTelemetry;
using OpenTelemetry.Logs;

namespace FluentdForward.OpenTelemetry.MessagePack.UnitTests;

[TestClass]
public class SerializeTests
{
	private readonly MessagePackSerializerOptions m_SerializerOptions = MessagePackSerializerOptions.Standard.WithResolver(CompositeResolver.Create(
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
			[
				default(IExternalScopeProvider),
				DateTime.Parse("2021-12-22T05:43:42.5945187Z"),
				"Test.Serialize",
				LogLevel.Information,
				new EventId(1),
				"aabbcc",
				new
				{
					m = "aabbcc"
				},
				default(Exception),
				default(IReadOnlyList<KeyValuePair<string, object>>)
			],
			null,
			null)!;

		var expected = "{\"CategoryName\":\"Test.Serialize\",\"EventId\":{\"Id\":1,\"Name\":null},\"FormattedMessage\":\"aabbcc\",\"LogLevel\":\"Information\",\"Timestamp\":\"2021-12-22T05:43:42.5945187Z\",\"TraceFlags\":\"None\"}";

		var bytes = global::MessagePack.MessagePackSerializer.Serialize(record, m_SerializerOptions);
		var actual = global::MessagePack.MessagePackSerializer.ConvertToJson(bytes);

		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void ArrayPayload_Serialize_Test()
	{
		var array = new ArrayPayload<string>
		{
			Tag = "ttt",
			Body = [
				new ArrayPayloadBody<string>{
					Metadata = new ArrayPayloadMetadata
					{
						Timestamp = DateTime.Parse("2021-12-22T05:43:42.5945187Z")
					},
					Message = "test1"
				},
				new ArrayPayloadBody<string>{
					Metadata = new ArrayPayloadMetadata
					{
						Timestamp = DateTime.Parse("2021-12-22T05:43:42.5945187Z")
					},
					Message = "test2"
				},
			]
		};

		var expected = "[\"ttt\",[[[1640151822,{}],\"test1\"],[[1640151822,{}],\"test2\"]]]";

		var bytes = global::MessagePack.MessagePackSerializer.Serialize(array, m_SerializerOptions);
		var actual = global::MessagePack.MessagePackSerializer.ConvertToJson(bytes);

		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void ArrayPayload_With_LogRecord_Serialize_Test()
	{
		var record = (LogRecord)Activator.CreateInstance(
			typeof(LogRecord),
			BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Instance,
			null,
			[
				default(IExternalScopeProvider),
				DateTime.Parse("2021-12-22T05:43:42.5945187Z"),
				"Test.Serialize",
				LogLevel.Information,
				new EventId(1),
				"aabbcc",
				new
				{
					m = "aabbcc"
				},
				default(Exception),
				new Dictionary<string, object?>
				{
					["m"] = "aabbcc"
				}.ToList()
			],
			null,
			null)!;

		var array = new ArrayPayload<LogRecord>
		{
			Tag = "ttt",
			Body = [
				new ArrayPayloadBody<LogRecord>{
					Metadata = new ArrayPayloadMetadata
					{
						Timestamp = DateTime.Parse("2021-12-22T05:43:42.5945187Z")
					},
					Message = record
				}
			]
		};

		var expected = "[\"ttt\",[[[1640151822,{}],{\"CategoryName\":\"Test.Serialize\",\"EventId\":{\"Id\":1,\"Name\":null},\"FormattedMessage\":\"aabbcc\",\"LogLevel\":\"Information\",\"Attributes\":[{\"m\":\"aabbcc\"}],\"Timestamp\":\"2021-12-22T05:43:42.5945187Z\",\"TraceFlags\":\"None\"}]]]";

		var bytes = global::MessagePack.MessagePackSerializer.Serialize(array, m_SerializerOptions);
		var actual = global::MessagePack.MessagePackSerializer.ConvertToJson(bytes);

		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void MessagePackSerializer_Serialize_Test()
	{
		var sut = new MessagePackSerializer();

		var batch = new Batch<LogRecord>([
			(LogRecord)Activator.CreateInstance(
			typeof(LogRecord),
			BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Instance,
			null,
			[
				default(IExternalScopeProvider),
				DateTime.Parse("2021-12-22T05:43:42.5945187Z"),
				"Test.Serialize",
				LogLevel.Information,
				new EventId(1),
				"aabbcc",
				new
				{
					m = "aabbcc"
				},
				default(Exception),
				default(IReadOnlyList<KeyValuePair<string, object>>)
			],
			null,
			null)!
		], 1);

		var expected = "[\"ttt\",[[[1640151822,{}],{\"CategoryName\":\"Test.Serialize\",\"EventId\":{\"Id\":1,\"Name\":null},\"FormattedMessage\":\"aabbcc\",\"LogLevel\":\"Information\",\"Timestamp\":\"2021-12-22T05:43:42.5945187Z\",\"TraceFlags\":\"None\"}]]]";

		var bytes = sut.Serialize("ttt", batch);
		var actual = global::MessagePack.MessagePackSerializer.ConvertToJson(bytes);

		Assert.AreEqual(expected, actual);
	}
}
