namespace FluentdForward.OpenTelemetry.MessagePack;

// 序列化由 ArrayPayloadFormatter<TMessage> 處理（IntKey 陣列格式），為 AOT 相容而不使用 [MessagePackObject]。
public class ArrayPayload<TMessage>
{
	/// <summary>
	/// Tag.
	/// </summary>
	public string Tag { get; set; } = default!;

	public IEnumerable<ArrayPayloadBody<TMessage>> Body { get; set; } = default!;
}
