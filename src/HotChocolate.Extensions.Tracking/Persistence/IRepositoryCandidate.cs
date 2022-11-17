using System;

namespace HotChocolate.Extensions.Tracking.Persistence
{
    public interface IRepositoryCandidate
    {
        ITrackingExporter Repository { get; }
        bool CanHandle(Type t);
    }
}
