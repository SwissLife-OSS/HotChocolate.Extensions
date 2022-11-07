using System;

namespace HotChocolate.Extensions.Tracking;

public interface ITrackingEntry
{
    public DateTimeOffset Date { get; }
}
