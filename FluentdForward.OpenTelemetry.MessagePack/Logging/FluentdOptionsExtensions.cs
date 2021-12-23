using FluentdForward.OpenTelemetry.Exporter.Logs;
using FluentdForward.OpenTelemetry.MessagePack;

namespace Microsoft.Extensions.Logging;

public static class FluentdOptionsExtensions
{
	public static void UseMessagePack(this FluentdOptions fluentdOptions, params MessagePack.IFormatterResolver[] formatterResolvers)
	{
		fluentdOptions.Serializer = new MessagePackSerializer(formatterResolvers);
	}
}
