using System.Diagnostics.Tracing;

namespace FluentdForward.OpenTelemetry.Exporter.Logs;

[EventSource(Name = "FluentdForward-OpenTelemetry-Exporter")]
internal class FluentdForwardExporterEventSource : EventSource
{
	public static readonly FluentdForwardExporterEventSource Log = new();

	[Event(1, Level = EventLevel.Informational, Keywords = Keywords.Startup)]
	public void ExportRegister()
	{
		if (IsEnabled(EventLevel.Informational, Keywords.Startup))
			WriteEvent(1, "FluentdForward export set.");
	}

	[Event(4, Message = "Unknown error in export method: {0}", Level = EventLevel.Error, Keywords = Keywords.Export)]
	public void ExportMethodException(Exception ex)
	{
		if (IsEnabled(EventLevel.Error, Keywords.Export))
			WriteEvent(4, ex.ToString());
	}

	public static class Keywords
	{
		public const EventKeywords Startup = (EventKeywords)0x0001;
		public const EventKeywords Export = (EventKeywords)0x0002;
	}
}
