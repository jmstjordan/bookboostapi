using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.DataContracts;
using System.Diagnostics;

public class ActivityEnricher : ITelemetryInitializer
{
    public void Initialize(ITelemetry telemetry)
    {
        var activity = Activity.Current;
        if (activity == null) return;

        // Only for RequestTelemetry (incoming HTTP requests)
        if (telemetry is RequestTelemetry requestTelemetry)
        {
            foreach (var tag in activity.Tags)
            {
                // Optionally: avoid overwriting existing properties
                if (!requestTelemetry.Properties.ContainsKey(tag.Key))
                {
                    requestTelemetry.Properties[tag.Key] = tag.Value;
                }
            }
        }

        // (Optional) Also for TraceTelemetry, DependencyTelemetry, etc. Add more cases if you want.
    }
}
