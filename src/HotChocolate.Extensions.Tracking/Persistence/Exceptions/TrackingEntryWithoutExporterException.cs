using System;

namespace HotChocolate.Extensions.Tracking.Persistence.Exceptions
{
    public class TrackingEntryWithoutExporterException : Exception
    {
        public TrackingEntryWithoutExporterException(Type type)
            : base($"No registered tracking exporter can handle " +
                  $"the tracking entry of type {type.Name}")
        {
            Type = type;
        }

        public Type Type { get; }
    }
}
