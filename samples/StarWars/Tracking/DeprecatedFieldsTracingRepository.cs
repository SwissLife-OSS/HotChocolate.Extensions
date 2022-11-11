using HotChocolate.Extensions.Tracking;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using HotChocolate.Extensions.Tracking.Persistence;

namespace StarWars.Tracking
{
    public class DeprecatedFieldsTracingRepository : ITrackingRepository
    {
        private readonly ILoggerFactory _loggerFactory;

        public DeprecatedFieldsTracingRepository(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public Task SaveTrackingEntryAsync(
            ITrackingEntry trackingEntry,
            CancellationToken cancellationToken)
        {
            _loggerFactory.CreateLogger<ITrackingEntry>()
                .LogInformation(JsonSerializer.Serialize(trackingEntry));
            return Task.CompletedTask;
        }
    }
}
