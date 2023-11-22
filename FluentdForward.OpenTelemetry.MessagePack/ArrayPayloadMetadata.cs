using MessagePack;

namespace FluentdForward.OpenTelemetry.MessagePack;

[MessagePackObject]
public class ArrayPayloadMetadata
{
	private static readonly Dictionary<string, object?> _EmptyAttributes = new();

	/// <summary>
	/// Timestamp.
	/// </summary>
	[Key(0)]
	[MessagePackFormatter(typeof(TimeStampFormatter))]
	public DateTime Timestamp { get; set; }

	[Key(1)]
	public Dictionary<string, object?> Attributes { get; set; } = _EmptyAttributes;
}
