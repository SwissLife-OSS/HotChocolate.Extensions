using System;

namespace HotChocolate.Extensions.Tracking.Persistence
{
    public interface ITrackingExporterFactory
    {
        ITrackingExporter Create(Type t);
    }
}
