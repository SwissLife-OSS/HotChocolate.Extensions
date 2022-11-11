using System;

namespace HotChocolate.Extensions.Tracking.Persistence
{
    internal class RepositoryCandidateForAll : IRepositoryCandidate
    {
        public RepositoryCandidateForAll(ITrackingRepository repository)
        {
            Repository = repository;
        }

        public ITrackingRepository Repository { get; }

        public bool CanHandle(Type t)
        {
            return true;
        }
    }
}
