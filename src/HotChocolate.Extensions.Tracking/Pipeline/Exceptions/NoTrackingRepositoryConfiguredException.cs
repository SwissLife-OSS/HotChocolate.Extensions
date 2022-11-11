using System;
using System.Runtime.Serialization;

namespace HotChocolate.Extensions.Tracking.Pipeline.Exceptions;

[Serializable]
public sealed class NoTrackingRepositoryConfiguredException : Exception
{
    public NoTrackingRepositoryConfiguredException()
        : base("No repository defined for storing the tracking messages")
    {
    }

    private NoTrackingRepositoryConfiguredException(
        SerializationInfo info,
        StreamingContext context)
        : base(info, context)
    {
    }
}
