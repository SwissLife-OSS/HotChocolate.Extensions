using System;

namespace HotChocolate.Extensions.Tracking.Persistence
{
    public interface IExporterCandidate
    {
        ITrackingExporter Exporter { get; }
        bool CanHandle(Type t);
    }
}
