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

        public ITrackingRepository Create<T>()
            where T : ITrackingEntry
        {
            IRepositoryCandidate? candidate = _candidates
                .OfType<RepositoryCandidate>()
                .FirstOrDefault(c => c.CanHandle<T>());

            candidate = candidate
                ?? _candidates.OfType<RepositoryCandidateForAll>().SingleOrDefault();

            if (candidate == null)
            {
                throw new TrackingEntryWithoutRepositoryException(typeof(T));
            }

            return candidate.Repository;
        }
    }

    public interface IRepositoryCandidate
    {
        ITrackingRepository Repository { get; }
        bool CanHandle<T>();
    }

    internal class RepositoryCandidateForAll : IRepositoryCandidate
    {
        public RepositoryCandidateForAll(ITrackingRepository repository)
        {
            Repository = repository;
        }

        public ITrackingRepository Repository { get; }

        public bool CanHandle<T>()
        {
            return true;
        }
    }

    internal class RepositoryCandidate : IRepositoryCandidate
    {
        private readonly IReadOnlyList<Type> _supportedTypes;

        public RepositoryCandidate(
            ITrackingRepository repository,
            IReadOnlyList<Type> supportedTypes)
        {
            Repository = repository;
            _supportedTypes = supportedTypes;
        }

        public ITrackingRepository Repository { get; }

        public bool CanHandle<T>()
        {
            return _supportedTypes.Contains(typeof(T));
        }
    }
}
