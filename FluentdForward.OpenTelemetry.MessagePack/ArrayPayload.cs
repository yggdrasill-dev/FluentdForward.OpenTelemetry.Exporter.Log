using MessagePack;

namespace FluentdForward.OpenTelemetry.MessagePack;

[MessagePackObject]
internal class ArrayPayload<TMessage>
{
	/// <summary>
	/// Tag.
	/// </summary>
	[Key(0)]
	public string Tag { get; set; } = default!;

	[Key(1)]
	public IEnumerable<ArrayPayloadBody<TMessage>> Body { get; set; } = default!;
}
