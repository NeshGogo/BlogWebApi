using OpenTelemetry;
using OpenTelemetry.Logs;
using System.Diagnostics;

namespace BlogWebApi;

public class ActivityEventLogProcessor : BaseProcessor<LogRecord>
{
    public override void OnEnd(LogRecord data)
    {
        base.OnEnd(data);
        var currentEvent = Activity.Current;
        currentEvent.AddEvent(new ActivityEvent(data.Attributes.ToString()));
    }
}
