using System;
using HotChocolate.Extensions.Tracking;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Http;

namespace StarWars.Tracking
{
    public class ReviewTrackingEntryFactory : ITrackingEntryFactory
    {
        public ITrackingEntry? CreateTrackingEntry(
            IHttpContextAccessor httpContextAccessor,
            IResolverContext context)
        {
            // you have access to a lot of things here: ClaimsPrincipal, Field Value etc...
            return new ReviewTrackingEntry(date: DateTime.Now);
        }
    }
}
