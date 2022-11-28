using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate.Extensions.Tracking.Persistence.Exceptions;
using HotChocolate.Types;

namespace HotChocolate.Extensions.Tracking.Persistence
{
    public class TrackingExporterFactory : ITrackingExporterFactory
    {
        private readonly IReadOnlyList<IExporterCandidate> _candidates;

        public TrackingExporterFactory(IReadOnlyList<IExporterCandidate> candidates)
        {
            _candidates = candidates;
        }

        public ITrackingExporter Create(
            Type t)
        {
            IExporterCandidate? candidate = _candidates
                .OfType<ExporterCandidate>()
                .FirstOrDefault(c => c.CanHandle(t));

            candidate = candidate
                ?? _candidates.OfType<ExporterCandidateForAll>().SingleOrDefault();

            if (candidate == null)
            {
                throw new TrackingEntryWithoutExporterException(t);
            }

            return candidate.Exporter;
        }
    }
}
