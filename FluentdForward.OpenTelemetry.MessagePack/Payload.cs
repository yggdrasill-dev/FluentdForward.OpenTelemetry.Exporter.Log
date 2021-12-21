using MessagePack;

namespace FluentdForward.OpenTelemetry.MessagePack;

/// <summary>
/// The class that defines a payload for sending to fluentd.
/// </summary>
[MessagePackObject]
internal class Payload<TMessage>
{
    /// <summary>
    /// Tag.
    /// </summary>
    [Key(0)]
    public string Tag { get; set; } = default!;

    /// <summary>
    /// Timestamp.
    /// </summary>
    [Key(1)]
    public long Timestamp { get; set; }

    /// <summary>
    /// Message.
    /// </summary>
    [Key(2)]
    public TMessage Message { get; set; } = default!;
}
