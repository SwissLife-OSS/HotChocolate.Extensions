using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Http;

namespace HotChocolate.Extensions.Tracking.FieldsLifetime
{
    public class DeprecatedFieldsTrackingEntryFactory : ITrackingEntryFactory
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
