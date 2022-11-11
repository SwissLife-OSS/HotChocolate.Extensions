using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate.Extensions.Tracking.Persistence.Exceptions;
using HotChocolate.Types;

namespace HotChocolate.Extensions.Tracking.Persistence
{
    public class TrackingRepositoryFactory : ITrackingRepositoryFactory
    {
        private readonly IReadOnlyList<IRepositoryCandidate> _candidates;

        public TrackingRepositoryFactory(IReadOnlyList<IRepositoryCandidate> candidates)
        {
            _candidates = candidates;
        }

        public ITrackingRepository Create(
            Type t)
        {
            IRepositoryCandidate? candidate = _candidates
                .OfType<RepositoryCandidate>()
                .FirstOrDefault(c => c.CanHandle(t));

            candidate = candidate
                ?? _candidates.OfType<RepositoryCandidateForAll>().SingleOrDefault();

            if (candidate == null)
            {
                throw new TrackingEntryWithoutRepositoryException(t);
            }

            return candidate.Repository;
        }
    }
}
