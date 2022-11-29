using System;
using System.Runtime.Serialization;

namespace HotChocolate.Extensions.Tracking.Pipeline.Exceptions;

[Serializable]
public sealed class NoTrackingExporterConfiguredException : Exception
{
    public NoTrackingExporterConfiguredException()
        : base("No exporter defined for storing the tracking messages")
    {
    }

    private NoTrackingExporterConfiguredException(
        SerializationInfo info,
        StreamingContext context)
        : base(info, context)
    {
    }
}
