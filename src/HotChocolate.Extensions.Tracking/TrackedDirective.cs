using System;

namespace HotChocolate.Extensions.Tracking;

public class TrackedDirective
{
    public TrackedDirective()
    {
    }

    public TrackedDirective(Func<IServiceProvider, ITrackingEntryFactory> getTrackingEntryFactory)
    {
        GetTrackingEntryFactory = getTrackingEntryFactory;
    }

    public TrackedDirective(ITrackingEntryFactory trackingEntryFactory)
    {
        GetTrackingEntryFactory = sp => trackingEntryFactory;
    }

    public Func<IServiceProvider, ITrackingEntryFactory> GetTrackingEntryFactory { get; }
}
