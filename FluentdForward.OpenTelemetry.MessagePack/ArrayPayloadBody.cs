using MessagePack;

namespace FluentdForward.OpenTelemetry.MessagePack;

[MessagePackObject]
public class ArrayPayloadBody<TMessage>
{
	/// <summary>
	/// Timestamp.
	/// </summary>
	[Key(0)]
	public ArrayPayloadMetadata Metadata { get; set; } = default!;

	/// <summary>
	/// Message.
	/// </summary>
	[Key(1)]
	public TMessage Message { get; set; } = default!;
}
