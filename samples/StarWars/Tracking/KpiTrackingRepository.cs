using HotChocolate.Extensions.Tracking.Persistence;
using HotChocolate.Extensions.Tracking;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Threading;

namespace StarWars.Tracking
{
    public class KpiTrackingRepository : ITrackingRepository
    {
        private readonly ILoggerFactory _loggerFactory;

        public KpiTrackingRepository(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public Task SaveTrackingEntryAsync(
            ITrackingEntry trackingEntry,
            CancellationToken cancellationToken)
        {
            _loggerFactory.CreateLogger<ITrackingEntry>()
                .LogInformation($"New KPI Value: {JsonConvert.SerializeObject(trackingEntry)}");
            return Task.CompletedTask;
        }
    }
}
