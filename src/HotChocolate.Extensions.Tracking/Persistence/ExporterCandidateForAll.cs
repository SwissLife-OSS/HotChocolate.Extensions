using System;

namespace HotChocolate.Extensions.Tracking.Persistence
{
    internal class ExporterCandidateForAll : IExporterCandidate
    {
        public ExporterCandidateForAll(ITrackingExporter exporter)
        {
            Exporter = exporter;
        }

        public ITrackingExporter Exporter { get; }

        public bool CanHandle(Type t)
        {
            return true;
        }
    }
}
