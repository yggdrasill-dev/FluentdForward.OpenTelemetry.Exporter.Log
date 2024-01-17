using System;
using System.Diagnostics.Tracing;
using System.Globalization;

namespace FluentdForward.OpenTelemetry.Exporter.Logs;

[EventSource(Name = "FluentdForward-OpenTelemetry-Exporter")]
internal class FluentdForwardExporterEventSource : EventSource
{
	public static readonly FluentdForwardExporterEventSource Log = new();

	[NonEvent]
	public void ExportMethodException(Exception ex)
	{
		if (IsEnabled(EventLevel.Error, Keywords.Export))
		{
			var originalUICulture = Thread.CurrentThread.CurrentUICulture;

			try
			{
				Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
				ExportMethodException(ex.ToString());
			}
			finally
			{
				Thread.CurrentThread.CurrentUICulture = originalUICulture;
			}
		}
	}

	[Event(1, Level = EventLevel.Informational, Keywords = Keywords.Startup, Message = "FluentdForward export set.")]
	public void ExportRegister()
	{
		if (IsEnabled(EventLevel.Informational, Keywords.Startup))
			WriteEvent(1);
	}

	[Event(4, Message = "Unknown error in export method: {0}", Level = EventLevel.Error, Keywords = Keywords.Export)]
	public void ExportMethodException(string ex)
	{
		WriteEvent(4, ex.ToString());
	}

	public static class Keywords
	{
		public const EventKeywords Startup = (EventKeywords)0x0001;
		public const EventKeywords Export = (EventKeywords)0x0002;
	}
}
