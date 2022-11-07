using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        IEnumerable<Claim> claims = httpContextAccessor.HttpContext.User.Claims;
        Claim? emailClaim = claims.Single(c => c.Type == "email");

        if (emailClaim.Value == null)
        {
            throw new NotSupportedException("Tracking needs the claim 'email'");
        }

        return new TrackingEntry(emailClaim.Value, DateTimeOffset.UtcNow, _tag);
    }
}
