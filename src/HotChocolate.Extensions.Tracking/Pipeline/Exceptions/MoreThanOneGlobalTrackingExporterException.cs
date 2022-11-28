using System;

namespace HotChocolate.Extensions.Tracking.Pipeline.Exceptions
{
    public class MoreThanOneGlobalTrackingExporterException : Exception
    {
        public MoreThanOneGlobalTrackingExporterException()
            : base("More than one exporter was declared without declaring its supported types.")
        {
        }
    }
}
