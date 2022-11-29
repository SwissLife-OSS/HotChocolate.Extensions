using System;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Http;

namespace HotChocolate.Extensions.Tracking.TagTracking;

public sealed class TagTrackingEntryFactory : ITrackingEntryFactory
{
    private readonly string _tag;

    internal TagTrackingEntryFactory(string tag)
    {
        _tag = tag;
    }

    public ITrackingEntry? CreateTrackingEntry(
        IHttpContextAccessor httpContextAccessor,
        IResolverContext context)
    {
        return new TagTrackingEntry(DateTimeOffset.UtcNow, _tag);
    }
}
