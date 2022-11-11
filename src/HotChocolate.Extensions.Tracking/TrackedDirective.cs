namespace HotChocolate.Extensions.Tracking;

public class TrackedDirective
{
    public TrackedDirective(ITrackingEntryFactory trackingEntryFactory)
    {
        TrackingEntryFactory = trackingEntryFactory;
    }

    public ITrackingEntryFactory TrackingEntryFactory { get; }
}
