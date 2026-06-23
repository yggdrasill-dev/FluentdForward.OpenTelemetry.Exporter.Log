using MessagePack;

namespace FluentdForward.OpenTelemetry.MessagePack;

// AOT-safe：以明確型別分支取代 MessagePackSerializer.Serialize(Type, ...) 的非泛型反射路徑
// （該路徑靠 Expression.Compile / Reflection.Emit，AOT 下會丟 PlatformNotSupportedException）。
// 涵蓋 OpenTelemetry log 屬性常見的純量型別；其餘型別退化為字串（ToString），確保 AOT 不會失敗。
internal static class MessagePackValueWriter
{
	public static void Write(ref MessagePackWriter writer, object? value)
	{
		switch (value)
		{
			case null: writer.WriteNil(); break;
			case string v: writer.Write(v); break;
			case bool v: writer.Write(v); break;
			case sbyte v: writer.Write(v); break;
			case byte v: writer.Write(v); break;
			case short v: writer.Write(v); break;
			case ushort v: writer.Write(v); break;
			case int v: writer.Write(v); break;
			case uint v: writer.Write(v); break;
			case long v: writer.Write(v); break;
			case ulong v: writer.Write(v); break;
			case float v: writer.Write(v); break;
			case double v: writer.Write(v); break;
			default: writer.Write(value.ToString()); break;
		}
	}
}
