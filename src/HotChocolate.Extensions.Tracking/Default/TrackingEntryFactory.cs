using System;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Http;

namespace HotChocolate.Extensions.Tracking.Default;

public sealed class TrackingEntryFactory : ITrackingEntryFactory
{
    private readonly string _tag;

    internal TrackingEntryFactory(string tag)
    {
        _tag = tag;
    }

    public ITrackingEntry? CreateTrackingEntry(
        IHttpContextAccessor httpContextAccessor,
        IResolverContext context)
    {
        return new TrackingEntry(DateTimeOffset.UtcNow, _tag);
    }
}
