namespace FluentdForward.OpenTelemetry.MessagePack;

// 序列化由 ArrayPayloadMetadataFormatter 處理（IntKey 陣列格式，Timestamp 走 TimestampFormatter），
// 為 AOT 相容而不使用 [MessagePackObject]。
public class ArrayPayloadMetadata
{
	private static readonly Dictionary<string, object?> _EmptyAttributes = new();

	/// <summary>
	/// Timestamp.
	/// </summary>
	public DateTime Timestamp { get; set; }

	public Dictionary<string, object?> Attributes { get; set; } = _EmptyAttributes;
}
