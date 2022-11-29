using System;

namespace HotChocolate.Extensions.Tracking.TagTracking;

public class TagTrackingEntry : ITrackingEntry
{
    public TagTrackingEntry(
        DateTimeOffset dateTimeOffset,
        string tag)
    {
        Date = dateTimeOffset;
        Tag = tag;
    }

    public DateTimeOffset Date { get; }
    public string Tag { get; }
}
