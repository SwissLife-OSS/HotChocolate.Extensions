using System;

namespace HotChocolate.Extensions.Tracking.Default;

public class TrackingEntry : ITrackingEntry
{
    public TrackingEntry(
        string userEmail,
        DateTimeOffset dateTimeOffset,
        string tag)
    {
        UserEmail = userEmail;
        Date = dateTimeOffset;
        Tag = tag;
    }

    public string UserEmail { get; }
    public DateTimeOffset Date { get; }
    public string Tag { get; }
}
