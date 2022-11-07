using System;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Http;

namespace HotChocolate.Extensions.Tracking.FieldsLifetime
{
    public class DeprecatedFieldsTrackingEntryFactory : IDeprecatedFieldsTrackingEntryFactory
    {
        public ITrackingEntry CreateTrackingEntry(
            IHttpContextAccessor httpContextAccessor,
            IResolverContext context)
        {
            return new DeprecatedFieldTrace(
                DateTimeOffset.UtcNow,
                context.Path);
        }
    }
}
