using System;

namespace HotChocolate.Extensions.Tracking.Persistence
{
    internal class RepositoryCandidateForAll : IRepositoryCandidate
    {
        public RepositoryCandidateForAll(ITrackingExporter repository)
        {
            Repository = repository;
        }

        public ITrackingExporter Repository { get; }

        public bool CanHandle(Type t)
        {
            return true;
        }
    }
}
