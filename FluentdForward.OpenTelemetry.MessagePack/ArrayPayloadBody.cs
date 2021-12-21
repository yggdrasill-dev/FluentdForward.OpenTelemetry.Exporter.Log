using MessagePack;

namespace FluentdForward.OpenTelemetry.MessagePack;

[MessagePackObject]
internal class ArrayPayloadBody<TMessage>
{
    /// <summary>
    /// Timestamp.
    /// </summary>
    [Key(0)]
    public long Timestamp { get; set; }

    /// <summary>
    /// Message.
    /// </summary>
    [Key(1)]
    public TMessage Message { get; set; } = default!;
}
