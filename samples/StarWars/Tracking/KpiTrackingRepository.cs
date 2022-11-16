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

        public async Task SaveTrackingEntryAsync(
            ITrackingEntry trackingEntry,
            CancellationToken cancellationToken)
        {
            await Task.Delay(5000); // Added some delay on the Repository to show that it is run asynchronously even after the end of the GraphQL Query
            _loggerFactory.CreateLogger<KpiTrackingRepository>()
                .LogInformation($"New KPI Value: {JsonConvert.SerializeObject(trackingEntry)}");
        }
    }
}
