namespace FluentdForward.OpenTelemetry.MessagePack;

// 序列化由 ArrayPayloadBodyFormatter<TMessage> 處理（IntKey 陣列格式），為 AOT 相容而不使用 [MessagePackObject]。
public class ArrayPayloadBody<TMessage>
{
	/// <summary>
	/// Timestamp.
	/// </summary>
	public ArrayPayloadMetadata Metadata { get; set; } = default!;

	/// <summary>
	/// Message.
	/// </summary>
	public TMessage Message { get; set; } = default!;
}
