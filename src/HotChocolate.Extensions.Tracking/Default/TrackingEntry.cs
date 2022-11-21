using System;

namespace HotChocolate.Extensions.Tracking.Default;

public class TrackingEntry : ITrackingEntry
{
    public TrackingEntry(
        DateTimeOffset dateTimeOffset,
        string tag)
    {
        Date = dateTimeOffset;
        Tag = tag;
    }

    public DateTimeOffset Date { get; }
    public string Tag { get; }
}
