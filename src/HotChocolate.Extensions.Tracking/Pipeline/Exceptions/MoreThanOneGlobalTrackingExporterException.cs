using System;
using System.Runtime.Serialization;

namespace HotChocolate.Extensions.Tracking.Pipeline.Exceptions
{
    public class MoreThanOneGlobalTrackingExporterException : Exception
    {
        public MoreThanOneGlobalTrackingExporterException()
            : base("More than one exporter was declared without declaring its supported types.")
        {
        }

        protected MoreThanOneGlobalTrackingExporterException(
            SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
