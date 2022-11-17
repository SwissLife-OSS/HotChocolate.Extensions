using System;
using System.Collections.Generic;
using System.Linq;

namespace HotChocolate.Extensions.Tracking.Persistence
{
    internal class RepositoryCandidate : IRepositoryCandidate
    {
        private readonly IReadOnlyList<Type> _supportedTypes;

        public RepositoryCandidate(
            ITrackingExporter repository,
            IReadOnlyList<Type> supportedTypes)
        {
            Repository = repository;
            _supportedTypes = supportedTypes;
        }

        public ITrackingExporter Repository { get; }

        public bool CanHandle(Type t)
        {
            return _supportedTypes.Contains(t);
        }
    }
}
