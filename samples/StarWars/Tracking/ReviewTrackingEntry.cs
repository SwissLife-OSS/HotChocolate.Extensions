using System;
using HotChocolate.Extensions.Tracking;

namespace StarWars.Tracking
{
    public class ReviewTrackingEntry : ITrackingEntry
    {
        public ReviewTrackingEntry(DateTimeOffset date)
        {
            Date = date;
        }

        public DateTimeOffset Date { get; }
    }
}
