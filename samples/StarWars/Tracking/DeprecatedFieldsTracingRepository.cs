using HotChocolate.Extensions.Tracking;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using HotChocolate.Extensions.Tracking.Persistence;
using Newtonsoft.Json;

namespace StarWars.Tracking
{
    public class DeprecatedFieldsTracingExporter : ITrackingExporter
    {
        private readonly ILoggerFactory _loggerFactory;

        public DeprecatedFieldsTracingExporter(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public Task SaveTrackingEntryAsync(
            ITrackingEntry trackingEntry,
            CancellationToken cancellationToken)
        {
            _loggerFactory.CreateLogger<DeprecatedFieldsTracingExporter>()
                .LogWarning($"Use of deprecated field: {JsonConvert.SerializeObject(trackingEntry)}");
            return Task.CompletedTask;
        }
    }
}
