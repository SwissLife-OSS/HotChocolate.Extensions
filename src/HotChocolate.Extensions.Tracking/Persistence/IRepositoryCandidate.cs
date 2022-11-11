using System;

namespace HotChocolate.Extensions.Tracking.Persistence
{
    public interface IRepositoryCandidate
    {
        ITrackingRepository Repository { get; }
        bool CanHandle(Type t);
    }
}
