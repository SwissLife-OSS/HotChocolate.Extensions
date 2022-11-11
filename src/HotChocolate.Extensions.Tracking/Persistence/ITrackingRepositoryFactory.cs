using System;

namespace HotChocolate.Extensions.Tracking.Persistence
{
    public interface ITrackingRepositoryFactory
    {
        ITrackingRepository Create(Type t);
    }
}
