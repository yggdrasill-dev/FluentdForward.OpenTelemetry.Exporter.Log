using System.Diagnostics.Tracing;

namespace FluentdForward.OpenTelemetry.Exporter.Logs;

internal class FluentdForwardExporterEventSource : EventSource
{
    public static readonly FluentdForwardExporterEventSource Log = new();

    [NonEvent]
    public void ExportMethodException(Exception ex)
    {
        if (Log.IsEnabled(EventLevel.Error, EventKeywords.All))
            WriteEvent(4, ex.ToString());
    }
}
