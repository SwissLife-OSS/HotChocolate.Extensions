using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Http;

namespace HotChocolate.Extensions.Tracking;

public interface ITrackingEntryFactory
{
    public ITrackingEntry? CreateTrackingEntry(
        IHttpContextAccessor httpContextAccessor,
        IResolverContext context);
}
