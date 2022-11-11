using HotChocolate.Extensions.Tracking;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using HotChocolate.Extensions.Tracking.Persistence;

namespace StarWars.Tracking
{
    public class DummyTrackingRepository : ITrackingRepository
    {
        private readonly ILogger _logger;

        public DummyTrackingRepository(ILogger logger)
        {
            _logger = logger;
        }

        public Task SaveTrackingEntryAsync(
            ITrackingEntry trackingEntry,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(JsonSerializer.Serialize(trackingEntry));
            return Task.CompletedTask;
        }
    }
}
