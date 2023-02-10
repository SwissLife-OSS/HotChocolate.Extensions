using System;
using System.Text.Json;
using HotChocolate.Extensions.Tracking;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using HotChocolate.Extensions.Tracking.Persistence;

namespace StarWars.Tracking
{
    public class DeprecatedFieldsTracingExporter : ITrackingExporter
    {
        private readonly ILoggerFactory _loggerFactory;

        public DeprecatedFieldsTracingExporter(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public async Task SaveTrackingEntryAsync(
            ITrackingEntry trackingEntry,
            CancellationToken cancellationToken)
        {
            // Added some delay on the Exporter to show that it is run asynchronously even after the end of the GraphQL Query
            await Task.Delay(5000, cancellationToken);

            _loggerFactory.CreateLogger<DeprecatedFieldsTracingExporter>()
                .LogWarning($"Use of deprecated field: {JsonSerializer.Serialize(trackingEntry)}");
        }
    }
}
