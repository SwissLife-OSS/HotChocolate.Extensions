using System;

namespace HotChocolate.Extensions.Tracking.FieldsLifetime
{
    public class DeprecatedFieldTrace : ITrackingEntry
    {
        public DeprecatedFieldTrace(
            DateTimeOffset date, Path path)
        {
            Date = date;
            Path = path;
        }

        public DateTimeOffset Date { get; }

        public Path Path { get; }
    }
}
