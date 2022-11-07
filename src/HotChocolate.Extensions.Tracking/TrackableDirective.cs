using System;

namespace HotChocolate.Extensions.Tracking;

public sealed class TrackableDirective
{
    public TrackableDirective(ITrackingEntryFactory trackingEntryFactory)
    {
        TrackingEntryFactory = trackingEntryFactory ??
            throw new ArgumentNullException(nameof(trackingEntryFactory));
    }

    public ITrackingEntryFactory TrackingEntryFactory { get; }
}
