namespace HotChocolate.Extensions.Tracking.Persistence;

public sealed class TrackingMessage
{
    public TrackingMessage(ITrackingEntry trackingEntry)
    {
        TrackingEntry = trackingEntry;
    }

    public ITrackingEntry TrackingEntry { get; }
}
