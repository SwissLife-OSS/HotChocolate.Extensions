namespace HotChocolate.Extensions.Tracking.Pipeline;

public sealed class TrackingMessage
{
    public TrackingMessage(ITrackingEntry trackingEntry)
    {
        TrackingEntry = trackingEntry;
    }

    public ITrackingEntry TrackingEntry { get; }
}
