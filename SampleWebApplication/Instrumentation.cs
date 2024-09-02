using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace SampleWebApplication;

public class Instrumentation : IDisposable
{
    static readonly AssemblyName Application = Assembly.GetEntryAssembly()!.GetName();
    internal static readonly string ActivitySourceName = Application.Name;
    internal static readonly string MeterName = Application.Name;
    private readonly Meter _meter;

    public Instrumentation()
    {
        string? version = typeof(Instrumentation).Assembly.GetName().Version?.ToString();
        ActivitySource = new ActivitySource(ActivitySourceName, version);
        _meter = new Meter(MeterName, version);
        HotDaysCounter = _meter.CreateCounter<long>("weather.days.hot", description: "The number of days where the temperature was really really hot");
    }

    public ActivitySource ActivitySource { get; }

    public Counter<long> HotDaysCounter { get; }

    public void Dispose()
    {
        ActivitySource.Dispose();
        _meter.Dispose();
    }
}
