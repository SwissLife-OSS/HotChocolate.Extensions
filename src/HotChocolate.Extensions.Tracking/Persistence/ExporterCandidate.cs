using System;
using System.Collections.Generic;
using System.Linq;

namespace HotChocolate.Extensions.Tracking.Persistence
{
    internal class ExporterCandidate : IExporterCandidate
    {
        private readonly IReadOnlyList<Type> _supportedTypes;

        public ExporterCandidate(
            ITrackingExporter exporter,
            IReadOnlyList<Type> supportedTypes)
        {
            Exporter = exporter;
            _supportedTypes = supportedTypes;
        }

        public ITrackingExporter Exporter { get; }

        public bool CanHandle(Type t)
        {
            return _supportedTypes.Contains(t);
        }
    }
}
